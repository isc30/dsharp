﻿// DelegateSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class DelegateSymbol : ITypeSymbol
    {
        public DelegateSymbol(string name, INamespaceSymbol parent)
            : base(SymbolType.Delegate, name, parent)
        {
        }
    }
}
