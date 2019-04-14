// ISymbolTable.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    public interface IScriptSymbolTable
    {
        IEnumerable<ISymbol> Symbols { get; }

        T FindSymbol<T>(string name, ISymbol context, SymbolFilter filter)
            where T : ISymbol;

        ISymbol FindSymbol(string name, ISymbol context, SymbolFilter filter);
    }
}
