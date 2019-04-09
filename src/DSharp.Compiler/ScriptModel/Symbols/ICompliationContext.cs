// SymbolSet.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;
using System.Xml;
using DSharp.Compiler.CodeModel;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    public interface ICompilationContext
    {
        IEnumerable<ScriptReference> Dependencies { get; }

        IMemberSymbol EntryPoint { get; }

        bool HasResources { get; }

        IScriptModel ScriptModel { get; }

        void AddDependency(ScriptReference dependency);

        ITypeSymbol CreateArrayTypeSymbol(ITypeSymbol itemTypeSymbol);

        ITypeSymbol CreateGenericTypeSymbol(ITypeSymbol templateType, IList<ITypeSymbol> typeArguments);

        ScriptReference GetDependency(string name, out bool newReference);

        string GetParameterDocumentation(string id, string paramName);

        Dictionary<string, ResXItem> GetResources(string name);

        string GetSummaryDocumentation(string id);

        bool IsSymbol(ITypeSymbol symbol, string symbolName);

        ITypeSymbol ResolveIntrinsicType(IntrinsicType type);

        ITypeSymbol ResolveType(ParseNode node, IScriptSymbolTable symbolTable, ISymbol contextSymbol);

        void SetComments(XmlDocument docComments);

        void SetEntryPoint(IMemberSymbol entryPoint);
    }
}
