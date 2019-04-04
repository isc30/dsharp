// Symbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections;
using System.Collections.Generic;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    public interface ISymbol
    {
        SymbolType Type { get; }

        string Name { get; }

        ISymbol Parent { get; }

        ICompilationContext SymbolSet { get; }

        string DocumentationId { get; }
    }

    public interface INamespaceSymbol : ISymbol
    {
        bool HasApplicationTypes { get; }

        ICollection<ITypeSymbol> Types { get; }

        void AddType(ITypeSymbol typeSymbol);

        void SetTransformedName(string name);
    }

    public interface ITypeSymbol : ISymbol
    {
        string FullGeneratedName { get; }

        string FullName { get; }

        string GeneratedNamespace { get; }

        IDictionary<string, string> Aliases { get; }

        ScriptReference Dependency { get; }

        ICollection<ITypeSymbol> GenericArguments { get; }

        bool IsApplicationType { get; }

        bool IsGeneric { get; }

        ICollection<GenericParameterSymbol> GenericParameters { get; }

        IEnumerable<IMemberSymbol> Members { get; }

        void AddMember(IMemberSymbol memberSymbol);

        bool IsArray { get; }

        INamespaceSymbol Namespace { get; }
    }

    public interface IMemberSymbol : ISymbol
    {
        ITypeSymbol AssociatedType { get; }
    }

    public class CompilationRoot : ICompilationRoot
    {
        private readonly IEnumerable<INamespaceSymbol> namespaceSymbols;

        public CompilationRoot(IEnumerable<INamespaceSymbol> namespaceSymbols)
        {
            this.namespaceSymbols = namespaceSymbols;
        }

        public IEnumerator<INamespaceSymbol> GetEnumerator()
        {
            return namespaceSymbols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return namespaceSymbols.GetEnumerator();
        }
    }

    public interface ICompilationRoot : IEnumerable<INamespaceSymbol>
    {

    }
}
