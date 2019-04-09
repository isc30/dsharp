// Expression.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.ScriptModel.Expressions
{
    internal abstract class Expression
    {
        protected Expression(ExpressionType type, ITypeSymbol evaluatedType, SymbolFilter memberMask)
        {
            Type = type;
            EvaluatedType = evaluatedType;
            MemberMask = memberMask | SymbolFilter.Members;
        }

        public ITypeSymbol EvaluatedType { get; private set; }

        protected virtual bool IsParenthesisRedundant => true;

        public SymbolFilter MemberMask { get; }

        public bool Parenthesized { get; private set; }

        public virtual bool RequiresThisContext => false;

        public ExpressionType Type { get; }

        public void AddParenthesisHint()
        {
            if (IsParenthesisRedundant == false)
            {
                Parenthesized = true;
            }
        }

        public void Reevaluate(ITypeSymbol evaluatedType)
        {
            EvaluatedType = evaluatedType;
        }
    }
}
