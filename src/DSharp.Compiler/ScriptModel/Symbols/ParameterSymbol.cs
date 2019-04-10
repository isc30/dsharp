// ParameterSymbol.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class ParameterSymbol : LocalSymbol
    {
        public ParameterSymbol(string name, IMemberSymbol parent, ITypeSymbol valueType, ParameterMode mode)
            : base(SymbolType.Parameter, name, parent, valueType)
        {
            Mode = mode;
        }

        public override string Documentation => Root.Documenation.GetParameterDocumentation(Parent.DocumentationId, Name);

        public override string DocumentationId
        {
            get
            {
                ITypeSymbol parameterType = ValueType;

                if (parameterType.IsArray)
                {
                    parameterType = ((ClassSymbol) parameterType).Indexer.AssociatedType;

                    return string.Format("{0}.{1}[]", parameterType.Namespace, parameterType.Name);
                }

                return string.Format("{0}.{1}", parameterType.Namespace, parameterType.Name);
            }
        }

        public ParameterMode Mode { get; }
    }
}
