// Symbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    public interface ISymbol : IScriptSymbolTable
    {
        SymbolType Type { get; }

        string Name { get; }

        string GeneratedName { get; }

        ISymbol Parent { get; }

        IScriptModel ScriptModel { get; }

        string Documentation { get; }

        string DocumentationId { get; }

        object ParseContext { get; }

        bool IsTransformAllowed { get; set; }

        bool IsTransformed { get; }

        bool MatchFilter(SymbolFilter filter);

        void SetTransformedName(string name);
    }

    public interface INamespaceSymbol : ISymbol
    {
        bool HasApplicationTypes { get; }

        IEnumerable<ITypeSymbol> Types { get; }

        void AddType(ITypeSymbol typeSymbol);
    }

    public interface ITypeSymbol : ISymbol
    {
        string FullGeneratedName { get; }
        string FullName { get; }
        string GeneratedNamespace { get; }
        bool IgnoreNamespace { get; set; }

        IDictionary<string, string> Aliases { get; }
        ScriptReference Dependency { get; }
        IEnumerable<string> Imports { get; }

        bool IsApplicationType { get; }
        bool IsArray { get; }
        bool IsCoreType { get; }
        bool IsGeneric { get; }
        bool IsPublic { get; }

        IEnumerable<ITypeSymbol> GenericArguments { get; }
        IEnumerable<IGenericParameterSymbol> GenericParameters { get; }
        ITypeSymbol GenericType { get; }

        IEnumerable<IMemberSymbol> Members { get; }
        INamespaceSymbol Namespace { get; }
        ITypeSymbol BaseType { get; }

        void AddMember(IMemberSymbol memberSymbol);
        IMemberSymbol GetMember(string name);
    }

    public interface IMemberSymbol : ISymbol
    {
        ITypeSymbol AssociatedType { get; }

        MemberVisibility Visibility { get; }

        void SetInterfaceMember(IMemberSymbol memberSymbol);
    }
}
