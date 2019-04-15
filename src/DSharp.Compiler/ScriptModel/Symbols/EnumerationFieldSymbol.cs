// EnumerationFieldSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class EnumerationFieldSymbol : FieldSymbol
    {
        public EnumerationFieldSymbol(string name, ITypeSymbol parent, object value, ITypeSymbol valueType)
            : base(SymbolType.EnumerationField, name, parent, valueType)
        {
            Visibility = MemberVisibility.Public | MemberVisibility.Static;
            Value = value;
        }
    }
}
