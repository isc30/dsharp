// MemberExpression.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.ScriptModel.Expressions
{
    internal sealed class MemberExpression : Expression
    {
        public MemberExpression(Expression objectReference, IMemberSymbol member)
            : base(ExpressionType.Member,
                member.AssociatedType.Type == SymbolType.GenericParameter
                    ? objectReference.EvaluatedType
                    : member.AssociatedType,
                SymbolFilter.Public | SymbolFilter.InstanceMembers)
        {
            Member = member;
            ObjectReference = objectReference;
        }

        public IMemberSymbol Member { get; }

        public Expression ObjectReference { get; }

        public override bool RequiresThisContext => ObjectReference.RequiresThisContext;
    }
}
