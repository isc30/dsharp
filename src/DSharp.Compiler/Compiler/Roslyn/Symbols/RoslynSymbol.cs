using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.ScriptModel;
using DSharp.Compiler.ScriptModel.Symbols;
using Microsoft.CodeAnalysis;

namespace DSharp.Compiler.Roslyn.Symbols
{
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

    public class RoslynNamespaceSymbol : RoslynSymbol<Microsoft.CodeAnalysis.INamespaceSymbol>, ScriptModel.Symbols.INamespaceSymbol
    {
        private readonly HashSet<ScriptModel.Symbols.ITypeSymbol> typeSymbols = new HashSet<ScriptModel.Symbols.ITypeSymbol>();

        public RoslynNamespaceSymbol(
            Microsoft.CodeAnalysis.INamespaceSymbol rootSymbol, 
            ISymbolContext symbolContext,
            ScriptModel.Symbols.ISymbol parent) 
            : base(rootSymbol, symbolContext, parent)
        {
        }

        public bool HasApplicationTypes => typeSymbols.Any(type => type.IsApplicationType);

        public IEnumerable<ScriptModel.Symbols.ITypeSymbol> Types => typeSymbols;

        public void AddType(ScriptModel.Symbols.ITypeSymbol typeSymbol)
        {
            if(typeSymbol == null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            typeSymbols.Add(typeSymbol);
        }

        public override bool MatchFilter(ScriptModel.Symbols.SymbolFilter filter)
            => (filter & DSharp.Compiler.ScriptModel.Symbols.SymbolFilter.Namespaces) != 0;
    }

    //Missing interface members;
    public class RoslynMemberSymbol : RoslynSymbol<Microsoft.CodeAnalysis.ISymbol>, IMemberSymbol
    {
        public ScriptModel.Symbols.ITypeSymbol AssociatedType => throw new NotImplementedException();

        public MemberVisibility Visibility => throw new NotImplementedException();

        public void SetInterfaceMember(IMemberSymbol memberSymbol)
        {
            throw new NotImplementedException();
        }
    }
}
