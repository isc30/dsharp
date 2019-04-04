﻿// Symbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Diagnostics;

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

        public virtual string Documentation => SymbolSet.GetSummaryDocumentation(DocumentationId);

        public virtual string DocumentationId
        {
            get
            {
                Debug.Fail("Documentation is not supported for this symbol type.");

                return null;
            }
        }

        public string Name { get; }

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

        public bool IsTransformAllowed { get; private set; }

        public bool IsTransformed => transformedName != null;

        public ISymbol Parent { get; }

        public object ParseContext
        {
            get
            {
                Debug.Assert(parseContext != null);

                return parseContext;
            }
        }

        public virtual ICompilationContext SymbolSet => Parent.SymbolSet;

        public SymbolType Type { get; }

        public void DisableNameTransformation()
        {
            Debug.Assert(IsTransformAllowed);

            IsTransformAllowed = false;
        }

        public abstract bool MatchFilter(SymbolFilter filter);

        public void SetTransformedName(string name)
        {
            Debug.Assert(IsTransformed == false);
            Debug.Assert(IsTransformAllowed);

            transformedName = name;
            IsTransformAllowed = false;
        }

        public void SetParseContext(object parseContext)
        {
            Debug.Assert(this.parseContext == null);
            Debug.Assert(parseContext != null);
            this.parseContext = parseContext;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
