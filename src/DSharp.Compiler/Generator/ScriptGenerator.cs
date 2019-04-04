﻿// ScriptGenerator.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.Generator
{
    internal sealed class ScriptGenerator
    {
        private readonly Stack<SymbolImplementation> implementationStack;

        public ScriptGenerator(TextWriter writer, CompilerOptions options)
        {
            Debug.Assert(writer != null);
            Writer = new ScriptTextWriter(writer);

            Options = options;
            implementationStack = new Stack<SymbolImplementation>();
        }

        public SymbolImplementation CurrentImplementation => implementationStack.Peek();

        public CompilerOptions Options { get; }

        public ScriptTextWriter Writer { get; }

        public void EndImplementation()
        {
            Debug.Assert(implementationStack.Count != 0);
            implementationStack.Pop();
        }

        public void GenerateScript(IScriptModel scriptModel)
        {
            Debug.Assert(scriptModel != null);

            List<TypeSymbol> types = new List<TypeSymbol>();
            List<TypeSymbol> publicTypes = new List<TypeSymbol>();
            List<TypeSymbol> internalTypes = new List<TypeSymbol>();

            bool hasNonModuleInternalTypes = false;

            foreach (NamespaceSymbol namespaceSymbol in scriptModel.Root)
                if (namespaceSymbol.HasApplicationTypes)
                {
                    foreach (TypeSymbol type in namespaceSymbol.Types)
                    {
                        if (type.IsApplicationType == false)
                        {
                            continue;
                        }

                        if (type.Type == SymbolType.Delegate)
                        {
                            // Nothing needs to be generated for delegate types.
                            continue;
                        }

                        if (type.Type == SymbolType.Enumeration &&
                            (type.IsPublic == false || ((EnumerationSymbol) type).Constants))
                        {
                            // Internal enums can be skipped since their values have been inlined.
                            // Public enums marked as constants can also be skipped since their
                            // values will always be inlined.
                            continue;
                        }

                        types.Add(type);

                        if (type.IsPublic)
                        {
                            publicTypes.Add(type);
                        }
                        else
                        {
                            if (type.Type != SymbolType.Class ||
                                ((ClassSymbol) type).IsModuleClass == false)
                            {
                                hasNonModuleInternalTypes = true;
                            }

                            internalTypes.Add(type);
                        }
                    }
                }

            // Sort the types, so similar types of types are grouped, and parent classes
            // come before derived classes.
            IComparer<TypeSymbol> typeComparer = new TypeComparer();
            types = types.OrderBy(t => t, typeComparer).ToList();
            publicTypes = publicTypes.OrderBy(t => t, typeComparer).ToList();
            internalTypes = internalTypes.OrderBy(t => t, typeComparer).ToList();

            bool initialIndent = false;

            if (string.IsNullOrEmpty(Options.ScriptInfo.Template) == false)
            {
                int scriptIndex = Options.ScriptInfo.Template.IndexOf("{script}");

                if (scriptIndex > 0 && Options.ScriptInfo.Template[scriptIndex - 1] == ' ')
                {
                    // Heuristic to turn on initial indent:
                    // The script template has a space prior to {script}, i.e. {script} is not the
                    // first thing on a line within the template.

                    initialIndent = true;
                }
            }

            if (initialIndent)
            {
                Writer.Indent++;
            }

            foreach (TypeSymbol type in types) TypeGenerator.GenerateScript(this, type);

            bool generateModule = publicTypes.Count != 0 ||
                                  internalTypes.Count != 0 && hasNonModuleInternalTypes;

            if (generateModule)
            {
                Writer.Write($"var $exports = {DSharpStringResources.ScriptExportMember("module")}('");
                Writer.Write(scriptModel.ScriptName);
                Writer.Write("',");

                if (internalTypes.Count != 0 && hasNonModuleInternalTypes)
                {
                    Writer.WriteLine();
                    Writer.Indent++;
                    Writer.WriteLine("{");
                    Writer.Indent++;
                    bool firstType = true;

                    foreach (TypeSymbol type in internalTypes)
                    {
                        if (type.Type == SymbolType.Class &&
                            (((ClassSymbol) type).IsExtenderClass || ((ClassSymbol) type).IsModuleClass))
                        {
                            continue;
                        }

                        if (type.Type == SymbolType.Record &&
                            ((RecordSymbol) type).Constructor == null)
                        {
                            continue;
                        }

                        if (firstType == false)
                        {
                            Writer.WriteLine(",");
                        }

                        TypeGenerator.GenerateRegistrationScript(this, type);
                        firstType = false;
                    }

                    Writer.Indent--;
                    Writer.WriteLine();
                    Writer.Write("},");
                    Writer.Indent--;
                }
                else
                {
                    Writer.Write(" null,");
                }

                if (publicTypes.Count != 0)
                {
                    Writer.WriteLine();
                    Writer.Indent++;
                    Writer.WriteLine("{");
                    Writer.Indent++;
                    bool firstType = true;

                    foreach (TypeSymbol type in publicTypes)
                    {
                        if (type.Type == SymbolType.Class &&
                            ((ClassSymbol) type).IsExtenderClass)
                        {
                            continue;
                        }

                        if (firstType == false)
                        {
                            Writer.WriteLine(",");
                        }

                        TypeGenerator.GenerateRegistrationScript(this, type);
                        firstType = false;
                    }

                    Writer.Indent--;
                    Writer.WriteLine();
                    Writer.Write("}");
                    Writer.Indent--;
                }
                else
                {
                    Writer.Write(" null");
                }

                Writer.WriteLine(");");
                Writer.WriteLine();
            }

            foreach (TypeSymbol type in types)
                if (type.Type == SymbolType.Class)
                {
                    TypeGenerator.GenerateClassConstructorScript(this, (ClassSymbol) type);
                }

            if (initialIndent)
            {
                Writer.Indent--;
            }
        }

        public void StartImplementation(SymbolImplementation implementation)
        {
            Debug.Assert(implementation != null);
            implementationStack.Push(implementation);
        }

        private sealed class TypeComparer : IComparer<TypeSymbol>
        {
            public int Compare(TypeSymbol x, TypeSymbol y)
            {
                if (x.Type != y.Type)
                {
                    // If types are different, then use the symbol type to
                    // similar types of types together.
                    return (int) x.Type - (int) y.Type;
                }

                if (x.Type == SymbolType.Class)
                {
                    // For classes, sort by inheritance depth. This is a crude
                    // way to ensure the base class for a class is generated
                    // first, without specifically looking at the inheritance
                    // chain for any particular type. A parent class with lesser
                    // inheritance depth has to by definition come first.

                    return ((ClassSymbol) x).InheritanceDepth - ((ClassSymbol) y).InheritanceDepth;
                }

                return 0;
            }
        }
    }
}
