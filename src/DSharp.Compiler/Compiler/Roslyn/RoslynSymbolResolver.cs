using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DSharp.Compiler.Roslyn
{
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
}
