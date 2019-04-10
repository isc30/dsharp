// LocalSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal abstract class LocalSymbol : Symbol
    {
        protected LocalSymbol(SymbolType type, string name, ISymbol parent, ITypeSymbol valueType)
            : base(type, name, parent)
        {
            ValueType = valueType;
        }

        public ITypeSymbol ValueType { get; }

        public override bool MatchFilter(SymbolFilter filter)
        {
            if ((filter & SymbolFilter.Locals) == 0)
            {
                return false;
            }

            return true;
        }
    }
}
