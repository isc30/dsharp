using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.Roslyn;
using DSharp.Compiler.Roslyn.Symbols;
using DSharp.Compiler.ScriptModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSharp.Compiler.Metadata
{
    public class RoslynScriptModelMetadataBuilder : IScriptModelBuilder<CSharpCompilation>
    {
        private List<ScriptModel.Symbols.ITypeSymbol> importedTypes = new List<ScriptModel.Symbols.ITypeSymbol>();
        private readonly ISymbolResolver symbolResolver = new RoslynSymbolResolver();

        public ICollection<ScriptModel.Symbols.ITypeSymbol> BuildMetadata(
            CSharpCompilation compilation,
            IScriptModel scriptModel,
            IScriptCompliationOptions options)
        {
            importedTypes.Clear();

            AssemblyBuildContext assemblyBuildContext = new AssemblyBuildContext
            {
                AssemblyName = options.AssemblyName
            };

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var rootNode = (CompilationUnitSyntax)syntaxTree.GetRoot();
                var rootUsings = rootNode.Usings.Select(us => us.Name.ResolveName()).ToArray();
                var namespaceDeclarations = rootNode.DescendantNodes().Where(node => node is NamespaceDeclarationSyntax);

                var typesAndNamespaces = semanticModel.LookupNamespacesAndTypes(0)
                    .Where(item => item.ContainingAssembly.Name != DSharpStringResources.DSHARP_MSCORLIB_ASSEMBLY_NAME)
                    .ToList();

                RoslynSymbolContext roslynSymbolContext = new RoslynSymbolContext
                {
                    BuildContext = assemblyBuildContext,
                    LocalSemanticModel = semanticModel,
                    ScriptModel = scriptModel,
                    Resolver = symbolResolver
                };

                ProcessSymbols(typesAndNamespaces, roslynSymbolContext);
            }

            return importedTypes;
        }

        private void ProcessSymbols(IEnumerable<ISymbol> symbols, ISymbolContext symbolContext)
        {
            foreach (var symbol in symbols)
            {
                if (symbol is INamespaceSymbol namespaceSymbol)
                {
                    string namespaceName = namespaceSymbol.ResolveFullNamespaceName();
                    var dns = symbolContext.ScriptModel.Namespaces.GetNamespace(namespaceName);
                    Console.WriteLine($"Visiting Namespace: {namespaceName}");
                    ProcessSymbols(namespaceSymbol.GetMembers(), symbolContext);
                }
                else if (symbol is ITypeSymbol typeSymbol)
                {
                    ProcessType(typeSymbol, symbolContext);
                }
                else
                {
                    Console.WriteLine($"Visiting Unknown of: {symbol.Kind}");
                    var missingSymbol = symbol.Kind;
                }
            }
        }

        private void ProcessType(ITypeSymbol typeSymbol, ISymbolContext symbolContext)
        {
            string typeName = typeSymbol.Name;
            var isPartial = typeSymbol.DeclaringSyntaxReferences.Length > 1;
            var usings = typeSymbol.DeclaringSyntaxReferences
                .SelectMany(node => (node.GetSyntax() as TypeDeclarationSyntax).GetAllUsings())
                .Concat(GetParentNamespacesForType(typeSymbol))
                .Distinct()
                .ToList();
            var aliases = typeSymbol.DeclaringSyntaxReferences
                .SelectMany(node => (node.GetSyntax() as TypeDeclarationSyntax).GetAllAliases())
                .Distinct()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Console.WriteLine($"Visiting Type: {typeName}, IsPartial: {isPartial}");

            var namespaceName = typeSymbol.ContainingNamespace.ResolveFullNamespaceName();
            var namespaceSymbol = symbolContext.ScriptModel.Namespaces.GetNamespace(namespaceName);

            var builtSymbol = new RoslynTypeSymbol(
                typeSymbol,
                symbolContext,
                namespaceSymbol,
                usings,
                aliases,
                isPartial);
        }

        //TODO: Optimise and get the parent namespace and explode into several strings instead of walking up the tree
        private static IEnumerable<string> GetParentNamespacesForType(ITypeSymbol typeSymbol)
        {
            List<string> namespaces = new List<string>();
            var currentNamespace = typeSymbol.ContainingNamespace;

            while (currentNamespace != null)
            {
                string namespaceName = currentNamespace.ResolveFullNamespaceName();
                if (string.IsNullOrEmpty(namespaceName))
                {
                    break;
                }

                namespaces.Add(namespaceName);
                currentNamespace = currentNamespace.ContainingNamespace;
            }

            return namespaces.Reverse<string>();
        }
    }
}
