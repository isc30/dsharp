using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DSharp.Compiler.Roslyn
{
    public interface ISymbolResolver
    {
        T Resolve<T>(ISymbol symbol)
            where T : ScriptModel.Symbols.ISymbol;

        IEnumerable<T> Resolve<T>(IEnumerable<ISymbol> symbols)
            where T : ScriptModel.Symbols.ISymbol;
    }
}
