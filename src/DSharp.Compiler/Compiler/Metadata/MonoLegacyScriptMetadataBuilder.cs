using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Attributes;
using DSharp.Compiler.CodeModel.Expressions;
using DSharp.Compiler.CodeModel.Names;
using DSharp.Compiler.CodeModel.Types;
using DSharp.Compiler.Errors;
using DSharp.Compiler.Extensions;
using DSharp.Compiler.ScriptModel;

namespace DSharp.Compiler.Metadata
{
    public class MonoLegacyScriptMetadataBuilder : IScriptMetadataBuilder<ParseNodeList>
    {
        private readonly IErrorHandler errorHandler;

        public MonoLegacyScriptMetadataBuilder(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public ScriptMetadata Build(ParseNodeList compilation, IScriptModel scriptModel, IScriptCompliationOptions options)
        {
            string scriptName = GetAssemblyScriptName(compilation) ?? options.AssemblyName;

            if (string.IsNullOrEmpty(scriptName))
            {
                errorHandler.ReportAssemblyError(scriptName, DSharpStringResources.ASSEMBLY_SCRIPT_ATTRIBUTE_MISSING);
            }
            else if (!Utility.IsValidScriptName(scriptName))
            {
                errorHandler.ReportAssemblyError(scriptName, string.Format(DSharpStringResources.INVALID_SCRIPT_NAME_FORMAT, scriptName));
            }

            ScriptMetadata scriptMetadata = new ScriptMetadata
            {
                ScriptName = scriptName
            };

            List<AttributeNode> referenceAttributes = GetAttributes(compilation, DSharpStringResources.SCRIPT_REFERENCE_ATTRIBUTE);

            foreach (AttributeNode attribNode in referenceAttributes)
            {
                string name = null;
                string identifier = null;
                string path = null;
                bool delayLoad = false;

                Debug.Assert(attribNode.Arguments.Count != 0 &&
                             attribNode.Arguments[0].NodeType == ParseNodeType.Literal);
                Debug.Assert(((LiteralNode)attribNode.Arguments[0]).Value is string);
                name = (string)((LiteralNode)attribNode.Arguments[0]).Value;

                if (attribNode.Arguments.Count > 1)
                {
                    for (int i = 1; i < attribNode.Arguments.Count; i++)
                    {
                        Debug.Assert(attribNode.Arguments[1] is BinaryExpressionNode);

                        BinaryExpressionNode propExpression = (BinaryExpressionNode)attribNode.Arguments[1];
                        Debug.Assert(propExpression.LeftChild.NodeType == ParseNodeType.Name);

                        string propName = ((NameNode)propExpression.LeftChild).Name;

                        if (string.CompareOrdinal(propName, "Identifier") == 0)
                        {
                            Debug.Assert(propExpression.RightChild.NodeType == ParseNodeType.Literal);
                            Debug.Assert(((LiteralNode)propExpression.RightChild).Value is string);

                            identifier = (string)((LiteralNode)propExpression.RightChild).Value;
                        }

                        if (string.CompareOrdinal(propName, "Path") == 0)
                        {
                            Debug.Assert(propExpression.RightChild.NodeType == ParseNodeType.Literal);
                            Debug.Assert(((LiteralNode)propExpression.RightChild).Value is string);

                            path = (string)((LiteralNode)propExpression.RightChild).Value;
                        }
                        else if (string.CompareOrdinal(propName, "DelayLoad") == 0)
                        {
                            Debug.Assert(propExpression.RightChild.NodeType == ParseNodeType.Literal);
                            Debug.Assert(((LiteralNode)propExpression.RightChild).Value is bool);

                            delayLoad = (bool)((LiteralNode)propExpression.RightChild).Value;
                        }
                    }
                }

                ScriptReference reference = scriptModel.GetDependency(name, out bool newReference);
                reference.Path = path;
                reference.DelayLoaded = delayLoad;

                if (newReference)
                {
                    reference.Identifier = identifier;
                }
            }

            if (GetScriptTemplate(compilation, out string template))
            {
                scriptMetadata.Template = template;
            }

            GetAssemblyMetadata(compilation, out string description, out string copyright, out string version);

            if (description != null)
            {
                scriptMetadata.Description = description;
            }

            if (copyright != null)
            {
                scriptMetadata.Copyright = copyright;
            }

            if (version != null)
            {
                scriptMetadata.Version = version;
            }

            return scriptMetadata;
        }

        private void GetAssemblyMetadata(ParseNodeList compilationUnits, out string description, out string copyright,
                                         out string version)
        {
            description = null;
            copyright = null;
            version = null;

            foreach (CompilationUnitNode compilationUnit in compilationUnits)
                foreach (AttributeBlockNode attribBlock in compilationUnit.Attributes)
                {
                    if (description == null)
                    {
                        description = attribBlock.Attributes.GetAttributeValue(nameof(AssemblyDescriptionAttribute));
                    }

                    if (copyright == null)
                    {
                        copyright = attribBlock.Attributes.GetAttributeValue(nameof(AssemblyCopyrightAttribute));
                    }

                    if (version == null)
                    {
                        version = attribBlock.Attributes.GetAttributeValue(nameof(AssemblyFileVersionAttribute));
                    }
                }
        }

        private string GetAssemblyScriptName(ParseNodeList compilationUnits)
        {
            foreach (CompilationUnitNode compilationUnit in compilationUnits)
                foreach (AttributeBlockNode attribBlock in compilationUnit.Attributes)
                {
                    string scriptName = attribBlock.Attributes.GetAttributeValue("ScriptAssembly");

                    if (scriptName != null)
                    {
                        return scriptName;
                    }
                }

            return null;
        }

        private List<AttributeNode> GetAttributes(ParseNodeList compilationUnits, string attributeName)
        {
            List<AttributeNode> attributes = new List<AttributeNode>();

            foreach (CompilationUnitNode compilationUnit in compilationUnits)
                foreach (AttributeBlockNode attribBlock in compilationUnit.Attributes)
                    foreach (AttributeNode attribNode in attribBlock.Attributes)
                        if (attribNode.TypeName.Equals(attributeName, StringComparison.Ordinal))
                        {
                            attributes.Add(attribNode);
                        }

            return attributes;
        }

        private bool GetScriptTemplate(ParseNodeList compilationUnits, out string template)
        {
            template = null;

            foreach (CompilationUnitNode compilationUnit in compilationUnits)
                foreach (AttributeBlockNode attribBlock in compilationUnit.Attributes)
                {
                    template = attribBlock.Attributes.GetAttributeValue("ScriptTemplate");

                    if (template != null)
                    {
                        return true;
                    }
                }

            return false;
        }
    }
}
