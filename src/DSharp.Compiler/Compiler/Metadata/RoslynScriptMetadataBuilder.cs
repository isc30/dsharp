using System;
using DSharp.Compiler.ScriptModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSharp.Compiler.Metadata
{
    public class RoslynScriptMetadataBuilder : IScriptMetadataBuilder<CSharpCompilation>
    {
        public ScriptMetadata Build(CSharpCompilation compilation, IScriptModel scriptModel, IScriptCompliationOptions options)
        {
            ScriptMetadataWalker scriptMetadataWalker = new ScriptMetadataWalker();
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                var semanticModel = compilation.GetSemanticModel(syntaxTree, false);
                scriptMetadataWalker.VisitWithSemanticModel(root, semanticModel);
            }

            if(!scriptMetadataWalker.HasAllRequiredMetadata)
            {
                throw new InvalidOperationException("Missing Required metadata");
            }

            return new ScriptMetadata
            {
                ScriptName = scriptMetadataWalker.ScriptAssemblyName,
                Template = scriptMetadataWalker.ScriptTemplate,
                Copyright = scriptMetadataWalker.AssemblyCopyright,
                Description = scriptMetadataWalker.AssemblyDescription,
                Version = scriptMetadataWalker.AssemblyVersion
            };
        }

        private class ScriptMetadataWalker : CSharpSyntaxWalker
        {
            private static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

            private SemanticModel model;

            public string ScriptAssemblyName { get; private set; }
            public string ScriptTemplate { get; private set; }
            public string AssemblyCopyright { get; private set; }
            public string AssemblyVersion { get; private set; }
            public string AssemblyDescription { get; private set; }

            public bool HasAllRequiredMetadata => !String.IsNullOrEmpty(ScriptAssemblyName);

            public ScriptMetadataWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
                : base(depth)
            {
            }

            public void VisitWithSemanticModel(SyntaxNode node, SemanticModel model)
            {
                this.model = model;
                Visit(node);
                this.model = null;
            }

            public override void VisitAttribute(AttributeSyntax node)
            {
                if (node.Name is IdentifierNameSyntax identifier)
                {
                    switch(identifier?.Identifier.ValueText)
                    {
                        case DSharpStringResources.SCRIPT_ASSEMBLY_ATTRIBUTE:
                            ScriptAssemblyName = GetFirstParameterValue(node);
                            break;
                        case DSharpStringResources.SCRIPT_TEMPLATE_ATTRIBUTE:
                            ScriptTemplate = GetFirstParameterValue(node);
                            break;
                        case "AssemblyCopyright":
                            AssemblyCopyright = GetFirstParameterValue(node);
                            break;
                        case "AssemblyDescription":
                            AssemblyDescription = GetFirstParameterValue(node);
                            break;
                        case "AssemblyFileVersion":
                            AssemblyVersion = GetFirstParameterValue(node);
                            break;
                    }
                }

                base.VisitAttribute(node);
            }

            private string GetFirstParameterValue(AttributeSyntax node)
            {
                var parameterExpressionNode = node.ArgumentList.Arguments.FirstOrDefault()?.Expression;

                if(parameterExpressionNode is LiteralExpressionSyntax literalExpression)
                {
                    return parameterExpressionNode?.GetText()?.ToString();
                }
                else if (parameterExpressionNode is MemberAccessExpressionSyntax memberAccessExpression)
                {
                    return (string)model.GetConstantValue(memberAccessExpression).Value;
                }

                return null;
            }
        }
    }
}
