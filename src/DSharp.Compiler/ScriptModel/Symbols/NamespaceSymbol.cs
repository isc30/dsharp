// NamespaceSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    public sealed class NamespaceSymbol : Symbol, INamespaceSymbol
    {
        private readonly Dictionary<string, ITypeSymbol> typeMap;
        private readonly List<ITypeSymbol> types;

        public NamespaceSymbol(string name, IScriptModel root)
            : base(SymbolType.Namespace, name, null)
        {
            Root = root;
            types = new List<ITypeSymbol>();
            typeMap = new Dictionary<string, ITypeSymbol>();
        }

        public bool HasApplicationTypes { get; private set; }

        public override IScriptModel Root { get; }

        public ICollection<ITypeSymbol> Types => types;

        public void AddType(ITypeSymbol typeSymbol)
        {
            Debug.Assert(typeSymbol != null);
            Debug.Assert(string.IsNullOrEmpty(typeSymbol.Name) == false);

            types.Add(typeSymbol);
            typeMap[typeSymbol.Name] = typeSymbol;

            if (typeSymbol.IsApplicationType)
            {
                HasApplicationTypes = true;
            }
        }

        public override bool MatchFilter(SymbolFilter filter)
        {
            if ((filter & SymbolFilter.Namespaces) == 0)
            {
                return false;
            }

            return true;
        }

        public override IEnumerable<ISymbol> Symbols => types;

        public override ISymbol FindSymbol(string name, ISymbol context, SymbolFilter filter)
        {
            Debug.Assert(string.IsNullOrEmpty(name) == false);
            Debug.Assert(context == null);
            Debug.Assert(filter == SymbolFilter.Types);

            if (typeMap.ContainsKey(name))
            {
                return typeMap[name];
            }

            return null;
        }
    }
}
