// LocalSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.ScriptModel.Expressions;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class VariableSymbol : LocalSymbol
    {
        public VariableSymbol(string name, IMemberSymbol parent, ITypeSymbol valueType)
            : base(SymbolType.Variable, name, parent, valueType)
        {
        }

        public Expression Value { get; private set; }

        public void SetValue(Expression value)
        {
            Value = value;
        }
    }
}
