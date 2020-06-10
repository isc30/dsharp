using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.ScriptModel.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharp.Compiler.Preprocessing.Lowering
{
    public class ExtensionMethodToStaticRewriter : CSharpSyntaxRewriter, ILowerer
    {
        private SemanticModel sem;
        private Dictionary<string, string> typeAliases = new Dictionary<string, string>();

        public CompilationUnitSyntax Apply(Compilation compilation, CompilationUnitSyntax root)
        {
            sem = compilation.GetSemanticModel(root.SyntaxTree);
            var newRoot = Visit(root) as CompilationUnitSyntax;

            if (typeAliases.Any())
            {
                var missingDirectives = typeAliases.Select(s => 
                    UsingDirective(
                        NameEquals(s.Key).WithLeadingTrivia(Whitespace(" ")),
                        ParseName(s.Value).WithLeadingTrivia(Whitespace(" "))
                    ).WithLeadingTrivia(CarriageReturn)
                ).ToArray();

                newRoot = newRoot.AddUsings(missingDirectives);
            }

            return newRoot;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symb = Try(() => sem.GetSymbolInfo(node).Symbol as IMethodSymbol, null);
            var newNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);

            if (symb != null
                && symb.IsExtensionMethod
                && symb.ReceiverType != symb.ContainingSymbol) // ignore extension methods invoked as static methods
            {
                var extensionClass = symb.ContainingSymbol.ToDisplayString();
                var extensionAlias = extensionClass.Replace(".", "_");

                if (!typeAliases.ContainsKey(extensionAlias))
                {
                    typeAliases.Add(extensionAlias, extensionClass);
                }

                switch (newNode.Expression)
                {
                    case MemberAccessExpressionSyntax memberExpression:
                        {
                            var newArguments = ArgumentList();
                            newArguments = newArguments.AddArguments(Argument(memberExpression.Expression.WithoutTrivia())); // extension method target
                            newArguments = newArguments.AddArguments(newNode.ArgumentList.Arguments.ToArray());

                            return InvocationExpression(
                                MemberAccessExpression(
                                    memberExpression.Kind(),
                                    IdentifierName(extensionAlias),
                                    memberExpression.OperatorToken,
                                    memberExpression.Name),
                                newArguments);
                        }

                    case SimpleNameSyntax nameExpression:
                        {
                            return InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(extensionAlias),
                                        nameExpression.WithoutTrivia()),
                                    newNode.ArgumentList);
                        }

                    default:
                        throw new NotImplementedException(); // please implement extra cases!
                }
            }

            return newNode;
        }

        private static T Try<T>(Func<T> action, T defaultValue)
        {
            try { return action(); }
            catch (Exception ex) { throw ex; }

            return defaultValue;
        }
    }
}
