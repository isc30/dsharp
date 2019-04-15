using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Types;
using DSharp.Compiler.Compiler;
using DSharp.Compiler.Errors;
using DSharp.Compiler.Generator;
using DSharp.Compiler.Importer;
using DSharp.Compiler.Metadata;
using DSharp.Compiler.ScriptModel;
using DSharp.Compiler.ScriptModel.Symbols;
using DSharp.Compiler.Validator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using ITypeSymbol = DSharp.Compiler.ScriptModel.Symbols.ITypeSymbol;

namespace DSharp.Compiler
{
    public sealed class ScriptCompiler
    {
        private readonly IErrorHandler errorHandler;

        private ParseNodeList compilationUnitList;
        private CompilerOptions options;
        private IScriptModel scriptModel;

        private ScriptMetadata ScriptMetadata => scriptModel.ScriptMetadata;

        private bool HasErrors => errorHandler.HasErrors;

        public ScriptCompiler()
            : this(null)
        {
        }

        public ScriptCompiler(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler ?? new ConsoleLoggingErrorHandler();
        }

        public bool Compile(CompilerOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));

            scriptModel = new ES5ScriptModel();

            var compilation = ImportMetadata();

            if (HasErrors)
            {
                return false;
            }

            compilation = BuildCodeModel(compilation);

            if (HasErrors)
            {
                return false;
            }

            BuildMetadata(compilation);

            if (HasErrors)
            {
                return false;
            }

            BuildImplementation();

            if (HasErrors)
            {
                return false;
            }

            GenerateScript();

            if (HasErrors)
            {
                return false;
            }

            return true;
        }

        private CSharpCompilation ImportMetadata()
        {
            var references = options.References.Select(r => MetadataReference.CreateFromFile(r));
            var compilationContext = CSharpCompilation.Create(options.AssemblyName)
                .AddReferences(references);

            MetadataImporter mdImporter = new MetadataImporter(errorHandler);

            mdImporter.ImportMetadata(options.References, scriptModel);
            return compilationContext;
        }

        private CSharpCompilation BuildCodeModel(CSharpCompilation compilation)
        {
            compilationUnitList = new ParseNodeList();

            CodeModelBuilder codeModelBuilder = new CodeModelBuilder(options, errorHandler);
            CodeModelValidator codeModelValidator = new CodeModelValidator(errorHandler);
            CodeModelProcessor validationProcessor = new CodeModelProcessor(codeModelValidator, options);

            foreach (IStreamSource source in options.Sources)
            {

                try
                {
                    CompilationUnitNode compilationUnit = codeModelBuilder.BuildCodeModel(source);

                    if (compilationUnit != null)
                    {
                        validationProcessor.Process(compilationUnit);

                        compilationUnitList.Append(compilationUnit);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Error occurred in File {source?.FullName}: {e.Message}, {e.StackTrace}  ");
                }
            }

            CSharpParseOptions cSharpParseOptions = new CSharpParseOptions(LanguageVersion.CSharp2)
                .WithPreprocessorSymbols(options.Defines)
                .WithKind(SourceCodeKind.Regular);

            foreach (var tree in options.Sources.Select(source =>
            {
                return CSharpSyntaxTree.ParseText(SourceText.From(source.GetStream()), cSharpParseOptions).GetRoot();
            }).ToList())
            {
                compilation = compilation.AddSyntaxTrees(tree.SyntaxTree);
            }

            return compilation;
        }

        private void BuildMetadata(CSharpCompilation cSharpCompilation)
        {
            if (options.Resources != null && options.Resources.Count != 0)
            {
                ResourcesBuilder resourcesBuilder = new ResourcesBuilder(scriptModel.Resources);
                resourcesBuilder.BuildResources(options.Resources);
            }

            IScriptMetadataBuilder<CSharpCompilation> roslynMetadataBuilder = new RoslynScriptMetadataBuilder();
            scriptModel.ScriptMetadata = roslynMetadataBuilder.Build(cSharpCompilation, scriptModel, options);
            scriptModel.ScriptMetadata.EnableDocComments = options.EnableDocComments;

            IScriptModelBuilder<ParseNodeList> mdBuilder = new MonoLegacyMetadataBuilder();
            IScriptModelBuilder<CSharpCompilation> roslynScriptModelBuilder = new RoslynScriptModelMetadataBuilder();

            var appSymbols = mdBuilder.BuildMetadata(compilationUnitList, scriptModel, options);
            var roslynAppSymbols = roslynScriptModelBuilder.BuildMetadata(cSharpCompilation, scriptModel, options);

            CheckForDuplicateTypes(appSymbols);

            //Is this step redundant now? We minimise after instead of before, because there are better tools to minimise
            ISymbolTransformer transformer = null;

            if (options.Minimize)
            {
                transformer = new SymbolObfuscator();
            }
            else
            {
                transformer = new SymbolInternalizer();
            }

            if (transformer != null)
            {
                SymbolSetTransformer symbolSetTransformer = new SymbolSetTransformer(transformer);
                ICollection<Symbol> transformedSymbols =
                    symbolSetTransformer.TransformSymbolSet(scriptModel, useInheritanceOrder: true);
            }
        }

        //Todo: Make this redundant by generating types with namespaces included
        private void CheckForDuplicateTypes(IEnumerable<ITypeSymbol> appSymbols)
        {
            HashSet<ITypeSymbol> types = new HashSet<ITypeSymbol>(new TypeSymbolComparer(t => t.GeneratedName));

            foreach (ITypeSymbol applicationSymbol in appSymbols)
            {
                if (applicationSymbol.ShouldSkipDuplicateTypeCheck())
                {

                    continue;
                }

                if (types.Contains(applicationSymbol))
                {
                    var existingType = types.First(sym => sym.GeneratedName == applicationSymbol.GeneratedName);
                    errorHandler.ReportGeneralError(string.Format(DSharpStringResources.CONFLICTING_TYPE_NAME_ERROR_FORMAT, applicationSymbol.FullName, existingType.FullName));
                }
                else
                {
                    types.Add(applicationSymbol);
                }
            }
        }

        //TODO: Replace this with an AST Tree.
        private void BuildImplementation()
        {
            CodeBuilder codeBuilder = new CodeBuilder(options, errorHandler);
            ICollection<SymbolImplementation> implementations = codeBuilder.BuildCode(scriptModel);

            if (options.Minimize)
            {
                foreach (SymbolImplementation impl in implementations)
                {
                    if (impl.Scope == null)
                    {
                        continue;
                    }

                    SymbolObfuscator obfuscator = new SymbolObfuscator();
                    SymbolImplementationTransformer transformer = new SymbolImplementationTransformer(obfuscator);

                    transformer.TransformSymbolImplementation(impl);
                }
            }
        }

        private void GenerateScript()
        {
            using (var outputStream = options.ScriptFile.GetStream())
            {
                if (outputStream == null)
                {
                    string scriptName = options.ScriptFile.FullName;
                    errorHandler.ReportMissingStreamError(scriptName);

                    return;
                }

                string script = GenerateScriptWithTemplate();

                using (var outputWriter = new StreamWriter(outputStream, Encoding.UTF8))
                {
                    outputWriter.Write(script);
                }
            }
        }

        private string GenerateScriptCore()
        {
            StringWriter scriptWriter = new StringWriter();

            try
            {
                ScriptGenerator scriptGenerator = new ScriptGenerator(scriptWriter, scriptModel.ScriptMetadata);
                scriptGenerator.GenerateScript(scriptModel);
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
            }
            finally
            {
                scriptWriter.Flush();
            }

            return scriptWriter.ToString();
        }

        private string GenerateScriptWithTemplate()
        {
            string script = GenerateScriptCore();

            string template = ScriptMetadata.Template;

            if (string.IsNullOrEmpty(template))
            {
                return script;
            }

            template = PreprocessTemplate(template);

            StringBuilder requiresBuilder = new StringBuilder();
            StringBuilder dependenciesBuilder = new StringBuilder();
            StringBuilder depLookupBuilder = new StringBuilder();

            bool firstDependency = true;

            foreach (ScriptReference dependency in scriptModel.Dependencies)
            {
                if (dependency.DelayLoaded)
                {
                    continue;
                }

                if (firstDependency)
                {
                    depLookupBuilder.Append("var ");
                }
                else
                {
                    requiresBuilder.Append(", ");
                    dependenciesBuilder.Append(", ");
                    depLookupBuilder.Append(",\r\n    ");
                }

                string name = dependency.Name;

                if (name == DSharpStringResources.DSHARP_SCRIPT_NAME)
                {
                    // TODO: This is a hack... to make generated node.js scripts
                    //       be able to reference the 'dsharp' node module.
                    //       Fix this in a better/1st class manner by allowing
                    //       script assemblies to declare such things.
                    name = DSharpStringResources.DSHARP_SCRIPT_NAME;
                }

                requiresBuilder.Append("'" + dependency.Path + "'");
                dependenciesBuilder.Append(dependency.Identifier);

                depLookupBuilder.Append(dependency.Identifier);
                depLookupBuilder.Append(" = require('" + name + "')");

                firstDependency = false;
            }

            depLookupBuilder.Append(";");

            return template.TrimStart()
                           .Replace("{name}", scriptModel.ScriptMetadata.ScriptName)
                           .Replace("{description}", ScriptMetadata.Description ?? string.Empty)
                           .Replace("{copyright}", ScriptMetadata.Copyright ?? string.Empty)
                           .Replace("{version}", ScriptMetadata.Version ?? string.Empty)
                           .Replace("{compiler}", typeof(ScriptCompiler).Assembly.GetName().Version.ToString())
                           .Replace("{description}", ScriptMetadata.Description)
                           .Replace("{requires}", requiresBuilder.ToString())
                           .Replace("{dependencies}", dependenciesBuilder.ToString())
                           .Replace("{dependenciesLookup}", depLookupBuilder.ToString())
                           .Replace("{script}", script);
        }

        private string PreprocessTemplate(string template)
        {
            if (options.IncludeResolver == null)
            {
                return template;
            }

            Regex includePattern = new Regex("\\{include:([^\\}]+)\\}",
                RegexOptions.Multiline | RegexOptions.CultureInvariant);

            return includePattern.Replace(template, delegate (Match include)
            {
                string includedScript = string.Empty;

                if (include.Groups.Count == 2)
                {
                    string includePath = include.Groups[1].Value;

                    IStreamSource includeSource = options.IncludeResolver.Resolve(includePath);

                    if (includeSource != null)
                    {
                        using (Stream includeStream = includeSource.GetStream())
                        {
                            StreamReader reader = new StreamReader(includeStream);

                            return reader.ReadToEnd();
                        }
                    }
                }

                return includedScript;
            });
        }
    }

    

    public class ConsoleLoggingErrorHandler : IErrorHandler
    {
        public bool HasErrors { get; private set; }

        public void ReportError(CompilerError error)
        {
            HasErrors = true;

            if (error.ColumnNumber != null || error.LineNumber != null)
            {
                Console.Error.WriteLine($"{error.File}({error.LineNumber.GetValueOrDefault()},{error.ColumnNumber.GetValueOrDefault()})");
            }

            Console.Error.WriteLine(error.Description);
        }
    }

    public static class MonoTypeSymbolExtensions
    {
        /// <summary>
        /// Skip the check for types that are marked as imported, as they
        /// aren't going to be generated into the script
        /// Delegates are implicitly imported types, as they're never generated into
        /// the script.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <returns></returns>
        public static bool ShouldSkipDuplicateTypeCheck(this ScriptModel.Symbols.ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsApplicationType == false
                || typeSymbol.Type == SymbolType.Delegate
                || (typeSymbol is ClassSymbol classSymbol && classSymbol.PrimaryPartialClass != typeSymbol);
        }
    }
}
