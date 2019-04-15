using System;
using System.Collections.Generic;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp
{
    public class TypeSymbolComparer : IEqualityComparer<ITypeSymbol>
    {
        private readonly Func<ITypeSymbol, object> comparerSelector;

        public TypeSymbolComparer(Func<ITypeSymbol, object> comparerSelector)
        {
            this.comparerSelector = comparerSelector ?? throw new ArgumentNullException(nameof(comparerSelector));
        }

        public bool Equals(ITypeSymbol x, ITypeSymbol y)
        {
            return comparerSelector.Invoke(x) == comparerSelector.Invoke(y);
        }

        public int GetHashCode(ITypeSymbol obj)
        {
            return comparerSelector.Invoke(obj)?.GetHashCode() ?? 0;
        }
    }
}
