// Symbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSharp.Compiler.ScriptModel.Symbols
{
    public abstract class Symbol : ISymbol
    {
        private object parseContext;
        private string transformedName;

        protected Symbol(SymbolType type, string name, ISymbol parent)
        {
            Type = type;
            Name = name;
            Parent = parent;
            IsTransformAllowed = true;
        }

        public virtual string Documentation => ScriptModel.Documenation.GetSummaryDocumentation(DocumentationId);

        public virtual string DocumentationId
        {
            get
            {
                Debug.Fail("Documentation is not supported for this symbol type.");

                return null;
            }
        }

        public virtual string Name { get; }

        public virtual string GeneratedName
        {
            get
            {
                if (IsTransformed)
                {
                    return transformedName;
                }

                return Name;
            }
        }

        public bool IsTransformAllowed { get; set; } = true;

        public bool IsTransformed => transformedName != null;

        public ISymbol Parent { get; }

        public object ParseContext { get; set; }

        public virtual IScriptModel ScriptModel => Parent.ScriptModel;

        public virtual SymbolType Type { get; }

        public virtual IEnumerable<ISymbol> Symbols { get; } = Enumerable.Empty<ISymbol>();

        public abstract bool MatchFilter(SymbolFilter filter);

        public void SetTransformedName(string name)
        {
            Debug.Assert(IsTransformed == false);
            Debug.Assert(IsTransformAllowed);

            transformedName = name;
            IsTransformAllowed = false;
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual ISymbol FindSymbol(string name, ISymbol context, SymbolFilter filter)
            => null;

        T IScriptSymbolTable.FindSymbol<T>(string name, ISymbol context, SymbolFilter filter)
            => (T)FindSymbol(name, context, filter);
    }
}
