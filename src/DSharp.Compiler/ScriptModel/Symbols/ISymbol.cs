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

        string GeneratedName { get; }

        ISymbol Parent { get; }

        IScriptModel Root { get; }

        string Documentation { get; }

        string DocumentationId { get; }

        bool MatchFilter(SymbolFilter filter);
    }

    public interface INamespaceSymbol : ISymbol
    {
        bool HasApplicationTypes { get; }

        ICollection<ITypeSymbol> Types { get; }

        void AddType(ITypeSymbol typeSymbol);

        void SetTransformedName(string name);
    }

    public interface ITypeSymbol : ISymbol, IScriptSymbolTable
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

        ITypeSymbol GenericType { get; }

        IMemberSymbol GetMember(string name);
    }

    public interface IMemberSymbol : ISymbol
    {
        ITypeSymbol AssociatedType { get; }

        MemberVisibility Visibility { get; }

        void SetInterfaceMember(IMemberSymbol memberSymbol);
    }
}
