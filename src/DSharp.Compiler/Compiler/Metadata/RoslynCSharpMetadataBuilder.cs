using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.ScriptModel;
using DSharp.Compiler.ScriptModel.Symbols;
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

        private void ProcessSymbols(IEnumerable<Microsoft.CodeAnalysis.ISymbol> symbols, ISymbolContext symbolContext)
        {
            foreach (var symbol in symbols)
            {
                if (symbol is Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol)
                {
                    string namespaceName = namespaceSymbol.ResolveFullNamespaceName();
                    var dns = symbolContext.ScriptModel.Namespaces.GetNamespace(namespaceName);
                    Console.WriteLine($"Visiting Namespace: {namespaceName}");
                    ProcessSymbols(namespaceSymbol.GetMembers(), symbolContext);
                }
                else if (symbol is Microsoft.CodeAnalysis.ITypeSymbol typeSymbol)
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

        private void ProcessType(Microsoft.CodeAnalysis.ITypeSymbol typeSymbol, ISymbolContext symbolContext)
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

            var namespaceSymbol = symbolContext.ScriptModel.Namespaces.GetNamespace(typeSymbol.ContainingNamespace.ResolveFullNamespaceName());

            var builtSymbol = new RoslynTypeSymbol(
                typeSymbol,
                symbolContext,
                namespaceSymbol,
                usings,
                aliases,
                isPartial);
        }

        //TODO: Optimise and get the parent namespace and explode into several strings instead of walking up the tree
        private static IEnumerable<string> GetParentNamespacesForType(Microsoft.CodeAnalysis.ITypeSymbol typeSymbol)
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

    internal class RoslynSymbolContext : ISymbolContext
    {
        public ISymbolResolver Resolver { get; set; }

        public IScriptModel ScriptModel { get; set; }

        public SemanticModel LocalSemanticModel { get; set; }

        public IAssemblyBuildContext BuildContext { get; set; }
    }

    internal class RoslynSymbolResolver : ISymbolResolver
    {
        public T Resolve<T>(Microsoft.CodeAnalysis.ISymbol symbol)
            where T : ScriptModel.Symbols.ISymbol
        {
            switch (symbol)
            {
                case IMethodSymbol method:
                    break;
                case Microsoft.CodeAnalysis.ITypeSymbol type:
                    break;
                case IPropertySymbol property:
                    break;
                case IAliasSymbol alias:
                    break;
                case IFieldSymbol field:
                    break;
                case IParameterSymbol parameter:
                    break;
                case ILocalSymbol local:
                    break;
                case IEventSymbol @event:
                    break;
            }

            return default;
        }

        public IEnumerable<T> Resolve<T>(IEnumerable<Microsoft.CodeAnalysis.ISymbol> symbols)
            where T : ScriptModel.Symbols.ISymbol
        {
            List<T> parsedSymbols = new List<T>();
            foreach (var symbol in symbols)
            {
                var parsedSymbol = Resolve<T>(symbol);
                if (parsedSymbol != null)
                {
                    parsedSymbols.Add(parsedSymbol);
                }
            }

            return parsedSymbols;
        }
    }

    internal class AssemblyBuildContext : IAssemblyBuildContext
    {
        public string AssemblyName { get; set; }
    }

    public interface ISymbolResolver
    {
        T Resolve<T>(Microsoft.CodeAnalysis.ISymbol symbol)
            where T : ScriptModel.Symbols.ISymbol;

        IEnumerable<T> Resolve<T>(IEnumerable<Microsoft.CodeAnalysis.ISymbol> symbols)
            where T : ScriptModel.Symbols.ISymbol;
    }

    public interface ISymbolContext
    {
        ISymbolResolver Resolver { get; }

        IScriptModel ScriptModel { get; }

        SemanticModel LocalSemanticModel { get; }

        IAssemblyBuildContext BuildContext { get; }
    }

    public interface IAssemblyBuildContext
    {
        string AssemblyName { get; }
    }

    public abstract class RoslynSymbol<T> : ScriptModel.Symbols.ISymbol
        where T : Microsoft.CodeAnalysis.ISymbol
    {
        private string transformedName;

        public RoslynSymbol(
            T rootSymbol,
            ISymbolContext symbolContext,
            ScriptModel.Symbols.ISymbol parent)
        {
            RootSymbol = rootSymbol;
            Context = symbolContext;
            Parent = parent;
        }

        protected T RootSymbol { get; }

        protected ISymbolContext Context { get; }

        protected ISymbolResolver SymbolResolver => Context?.Resolver;

        public SymbolType Type => RootSymbol.ResolveSymbolType();

        public string Name => RootSymbol.Name;
        public string GeneratedName => IsTransformed ? transformedName : Name;

        public ScriptModel.Symbols.ISymbol Parent { get; }
        public IScriptModel ScriptModel => Context.ScriptModel;
        public object ParseContext => RootSymbol;

        public bool IsTransformAllowed { get; set; }
        public bool IsTransformed => false;

        public string Documentation => RootSymbol.GetDocumentationCommentXml();
        public string DocumentationId => RootSymbol.GetDocumentationCommentId();

        public virtual IEnumerable<ScriptModel.Symbols.ISymbol> Symbols { get; }

        public void SetTransformedName(string transformedName)
        {
            this.transformedName = transformedName ?? throw new ArgumentNullException(nameof(transformedName));
        }

        public virtual ScriptModel.Symbols.ISymbol FindSymbol(string name, ScriptModel.Symbols.ISymbol context, ScriptModel.Symbols.SymbolFilter filter)
            => null;

        T IScriptSymbolTable.FindSymbol<T>(string name, ScriptModel.Symbols.ISymbol context, ScriptModel.Symbols.SymbolFilter filter)
            => (T)FindSymbol(name, context, filter);

        public abstract bool MatchFilter(ScriptModel.Symbols.SymbolFilter filter);
    }

    public class RoslynTypeSymbol : RoslynSymbol<Microsoft.CodeAnalysis.ITypeSymbol>, ScriptModel.Symbols.ITypeSymbol
    {
        private readonly bool isPartial;

        private readonly Lazy<IEnumerable<IMemberSymbol>> memberSymbols;
        private readonly Lazy<ScriptModel.Symbols.INamespaceSymbol> namespaceSymbol;
        private readonly Lazy<ScriptModel.Symbols.ITypeSymbol> baseTypeSymbol;

        public RoslynTypeSymbol(
            Microsoft.CodeAnalysis.ITypeSymbol rootSymbol,
            ISymbolContext symbolContext,
            ScriptModel.Symbols.ISymbol parent,
            IEnumerable<string> usings,
            Dictionary<string, string> aliases, bool isPartial)
            : base(rootSymbol, symbolContext, parent)
        {
            this.isPartial = isPartial;
            Imports = usings;
            Aliases = aliases;

            memberSymbols = new Lazy<IEnumerable<IMemberSymbol>>(() =>
            {
                return SymbolResolver.Resolve<IMemberSymbol>(rootSymbol.GetMembers());
            });

            namespaceSymbol = new Lazy<ScriptModel.Symbols.INamespaceSymbol>(() =>
            {
                return rootSymbol.ResolveScriptNamespace(ScriptModel);
            });

            baseTypeSymbol = new Lazy<ScriptModel.Symbols.ITypeSymbol>(() =>
            {
                return SymbolResolver.Resolve<ScriptModel.Symbols.ITypeSymbol>(RootSymbol.BaseType);
            });
        }

        public bool IgnoreNamespace { get; set; } = false;
        public string FullGeneratedName => IgnoreNamespace ? GeneratedName : GeneratedNamespace + "." + GeneratedName;
        public string GeneratedNamespace => Namespace.GeneratedName;
        public string FullName => RootSymbol.ContainingNamespace.ResolveFullNamespaceName() + "." + Name;

        public ScriptModel.Symbols.INamespaceSymbol Namespace => namespaceSymbol.Value;
        public IEnumerable<IMemberSymbol> Members => memberSymbols.Value;
        public override IEnumerable<ScriptModel.Symbols.ISymbol> Symbols => memberSymbols.Value;
        public ScriptModel.Symbols.ITypeSymbol BaseType => baseTypeSymbol.Value;

        public IDictionary<string, string> Aliases { get; }
        public IEnumerable<string> Imports { get; private set; }
        public ScriptReference Dependency => throw new NotImplementedException();

        public bool IsApplicationType => RootSymbol.ContainingModule.Name == Context.BuildContext.AssemblyName;
        public bool IsGeneric => RootSymbol.GetTypeMembers().Any(m => m.IsGenericType);
        public bool IsArray => RootSymbol.Kind == SymbolKind.ArrayType; // Does this work?
        public bool IsPublic => RootSymbol.DeclaredAccessibility == Accessibility.Public;
        public bool IsCoreType => RootSymbol.ContainingModule.Name == DSharpStringResources.DSHARP_MSCORLIB_ASSEMBLY_NAME;

        public ScriptModel.Symbols.ITypeSymbol GenericType => throw new NotImplementedException();
        public IEnumerable<ScriptModel.Symbols.ITypeSymbol> GenericArguments => throw new NotImplementedException();
        public IEnumerable<IGenericParameterSymbol> GenericParameters => SymbolResolver.Resolve<IGenericParameterSymbol>(RootSymbol.GetTypeMembers());

        public void AddMember(IMemberSymbol memberSymbol)
        {
            throw new NotImplementedException();
        }

        public IMemberSymbol GetMember(string name)
        {
            throw new NotImplementedException();
        }

        public override bool MatchFilter(ScriptModel.Symbols.SymbolFilter filter)
            => (filter & DSharp.Compiler.ScriptModel.Symbols.SymbolFilter.Types) != 0;
    }
}

namespace DSharp.Compiler
{
    public static class RoslynExtensions
    {
        public static string ResolveFullNamespaceName(this Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol)
        {
            List<string> partialNames = new List<string>()
            {
                namespaceSymbol.Name
            };

            while (namespaceSymbol.ContainingNamespace != null)
            {
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
                if (string.IsNullOrEmpty(namespaceSymbol.Name))
                {
                    break;
                }

                partialNames.Add(namespaceSymbol.Name);
            }

            return string.Join(".", partialNames.Reverse<string>());
        }

        public static string ResolveName(this NameSyntax nameSyntax)
        {
            if (nameSyntax is IdentifierNameSyntax identifier)
            {
                return identifier.Identifier.ValueText;
            }
            else if (nameSyntax is QualifiedNameSyntax qualifiedName)
            {
                return ResolveName(qualifiedName.Left) + qualifiedName.DotToken.ValueText + ResolveName(qualifiedName.Right);
            }
            else if (nameSyntax is SimpleNameSyntax simpleName)
            {
                return simpleName.Identifier.ValueText;
            }

            return string.Empty;
        }

        public static IEnumerable<string> GetAllUsings(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.ResolveTypeUsings(us =>
            {
                return us.Where(item => item.Alias == null)
                    .Select(item => item.Name.ResolveName());
            });
        }

        public static IEnumerable<KeyValuePair<string, string>> GetAllAliases(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.ResolveTypeUsings(us =>
            {
                return us.Where(item => item.Alias != null).Select(item =>
                {
                    string alias = item.Name.ResolveName();
                    string typeName = item.Alias.Name.ResolveName();
                    return new KeyValuePair<string, string>(alias, typeName);
                });
            });
        }

        private static IEnumerable<T> ResolveTypeUsings<T>(
            this TypeDeclarationSyntax typeDeclarationSyntax, 
            Func<SyntaxList<UsingDirectiveSyntax>, IEnumerable<T>> resolver)
        {
            if (typeDeclarationSyntax == null)
            {
                return Enumerable.Empty<T>();
            }

            List<T> allResolved = new List<T>();
            var currentNode = typeDeclarationSyntax.Parent;

            while (currentNode != null)
            {
                if (currentNode is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    var resolved = resolver.Invoke(namespaceDeclaration.Usings);
                    allResolved.AddRange(resolved);
                }
                else if (currentNode is CompilationUnitSyntax compilationUnitSyntax)
                {
                    var resolved = resolver.Invoke(compilationUnitSyntax.Usings);
                    allResolved.AddRange(resolved);
                }
                else
                {
                    Console.Error.WriteLine($"Unknown parent of type node '{currentNode.Kind()}' while resolving usings");
                }

                currentNode = currentNode.Parent;
            }

            return allResolved;
        }
    }

    public static class RoslynToScriptExtensions
    {
        public static SymbolType ResolveSymbolType(this Microsoft.CodeAnalysis.ISymbol symbol)
        {
            if (symbol is Microsoft.CodeAnalysis.ITypeSymbol typeSymbol)
            {
                switch (typeSymbol.TypeKind)
                {
                    case TypeKind.Class:
                        return SymbolType.Class;
                    case TypeKind.Delegate:
                        return SymbolType.Delegate;
                    case TypeKind.Enum:
                        return SymbolType.Enumeration;
                    case TypeKind.Interface:
                        return SymbolType.Interface;
                    case TypeKind.Struct:
                        return SymbolType.Class;
                    case TypeKind.TypeParameter:
                        return SymbolType.GenericParameter;
                    case TypeKind.Array:
                    case TypeKind.Dynamic:
                    case TypeKind.Error:
                    case TypeKind.Module:
                    case TypeKind.Pointer:
                    case TypeKind.Submission:
                    case TypeKind.Unknown:
                        break;
                }
            }

            if (symbol is IEventSymbol)
            {
                return SymbolType.Event;
            }

            if (symbol is IMethodSymbol methodSymbol)
            {
                switch (methodSymbol.MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.StaticConstructor:
                        return SymbolType.Constructor;
                    case MethodKind.LambdaMethod:
                    case MethodKind.DelegateInvoke:
                        return SymbolType.AnonymousMethod;
                    case MethodKind.Ordinary:
                    case MethodKind.ExplicitInterfaceImplementation:
                        return SymbolType.Method;
                }
            }

            switch (symbol.Kind)
            {
                case SymbolKind.Namespace:
                    return SymbolType.Namespace;
                case SymbolKind.Field:
                    return SymbolType.Field;
                case SymbolKind.Property:
                    return SymbolType.Property;
                case SymbolKind.Event:
                    return SymbolType.Event;
                case SymbolKind.Parameter:
                    return SymbolType.Parameter;
                case SymbolKind.TypeParameter:
                    return SymbolType.GenericParameter;
            }

            return SymbolType.Unknown;
        }

        public static ScriptModel.Symbols.INamespaceSymbol ResolveScriptNamespace(this Microsoft.CodeAnalysis.ISymbol symbol, IScriptModel scriptModel)
        {
            string namespaceName = symbol.ContainingNamespace.ResolveFullNamespaceName();
            return scriptModel.Namespaces.GetNamespace(namespaceName);
        }

        
    }
}
