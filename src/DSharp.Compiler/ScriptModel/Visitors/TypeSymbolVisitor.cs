using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.ScriptModel.Visitors
{
    internal abstract class BaseSymbolVisitor
    {
        protected virtual void VisitTypeSymbol(TypeSymbol type)
        {
            if (type is ClassSymbol classSymbol)
            {
                VisitClassSymbol(classSymbol);
            }
            else if (type is InterfaceSymbol interfaceSymbol)
            {
                VisitInterfaceSymbol(interfaceSymbol);
            }
        }

        protected virtual void VisitClassSymbol(ClassSymbol classSymbol)
        {
            foreach (var extendedInterfaceSymbol in classSymbol?.Interfaces ?? Enumerable.Empty<InterfaceSymbol>())
            {
                VisitTypeSymbol(extendedInterfaceSymbol);
            }

            TypeSymbol baseType = classSymbol.GetBaseType();

            if (baseType != null)
            {
                VisitTypeSymbol(baseType);
            }
        }

        protected virtual void VisitInterfaceSymbol(InterfaceSymbol interfaceSymbol)
        {
            foreach (var extendedInterfaceSymbol in interfaceSymbol?.Interfaces ?? Enumerable.Empty<InterfaceSymbol>())
            {
                VisitTypeSymbol(extendedInterfaceSymbol);
            }
        }
    }

    internal sealed class TypeSymbolVisitor<T> : BaseSymbolVisitor
    {
        private readonly Func<TypeSymbol, T> selector;
        private readonly List<T> items = new List<T>();

        private TypeSymbolVisitor(Func<TypeSymbol, T> selector)
        {
            this.selector = selector;
        }

        public static IList<T> Visit(TypeSymbol type, Func<TypeSymbol, T> selector)
        {
            TypeSymbolVisitor<T> visitor = new TypeSymbolVisitor<T>(selector);
            visitor.VisitTypeSymbol(type);

            return visitor.items;
        }

        protected override void VisitTypeSymbol(TypeSymbol type)
        {
            items.Add(selector(type));

            base.VisitTypeSymbol(type);
        }
    }
}
