// IndexerSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Diagnostics;
using System.Text;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class IndexerSymbol : PropertySymbol
    {
        public IndexerSymbol(ITypeSymbol parent, ITypeSymbol propertyType)
            : base(SymbolType.Indexer, "Item", parent, propertyType)
        {
        }

        public IndexerSymbol(ITypeSymbol parent, ITypeSymbol propertyType, MemberVisibility visibility)
            : this(parent, propertyType)
        {
            Visibility = visibility;
        }

        public override string DocumentationId
        {
            get
            {
                ITypeSymbol parent = (ITypeSymbol) Parent;

                StringBuilder sb = new StringBuilder();
                sb.Append("P:");
                sb.Append(parent.Namespace);
                sb.Append(".");

                // Only include the first parameter.
                sb.Append(parent.Name);
                sb.Append(".Item(");
                sb.Append(Parameters[0].DocumentationId);
                sb.Append(")");

                return sb.ToString();
            }
        }

        public bool UseScriptIndexer { get; private set; }

        public void SetScriptIndexer()
        {
            Debug.Assert(UseScriptIndexer == false);
            UseScriptIndexer = true;
        }
    }
}
