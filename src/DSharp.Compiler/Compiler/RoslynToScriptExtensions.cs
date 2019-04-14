using DSharp.Compiler.ScriptModel;
using DSharp.Compiler.ScriptModel.Symbols;
using Microsoft.CodeAnalysis;

namespace DSharp.Compiler
{
    public static class RoslynToScriptExtensions
    {
        public static SymbolType ResolveSymbolType(this Microsoft.CodeAnalysis.ISymbol symbol)
        {
            if (symbol is Microsoft.CodeAnalysis.ITypeSymbol typeSymbol)
            {
                switch (typeSymbol.TypeKind)
                {
                    case TypeKind.Class:
                        return SymbolType.Class;
                    case TypeKind.Delegate:
                        return SymbolType.Delegate;
                    case TypeKind.Enum:
                        return SymbolType.Enumeration;
                    case TypeKind.Interface:
                        return SymbolType.Interface;
                    case TypeKind.Struct:
                        return SymbolType.Class;
                    case TypeKind.TypeParameter:
                        return SymbolType.GenericParameter;
                    case TypeKind.Array:
                    case TypeKind.Dynamic:
                    case TypeKind.Error:
                    case TypeKind.Module:
                    case TypeKind.Pointer:
                    case TypeKind.Submission:
                    case TypeKind.Unknown:
                        break;
                }
            }

            if (symbol is IEventSymbol)
            {
                return SymbolType.Event;
            }

            if (symbol is IMethodSymbol methodSymbol)
            {
                switch (methodSymbol.MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.StaticConstructor:
                        return SymbolType.Constructor;
                    case MethodKind.LambdaMethod:
                    case MethodKind.DelegateInvoke:
                        return SymbolType.AnonymousMethod;
                    case MethodKind.Ordinary:
                    case MethodKind.ExplicitInterfaceImplementation:
                        return SymbolType.Method;
                }
            }

            switch (symbol.Kind)
            {
                case SymbolKind.Namespace:
                    return SymbolType.Namespace;
                case SymbolKind.Field:
                    return SymbolType.Field;
                case SymbolKind.Property:
                    return SymbolType.Property;
                case SymbolKind.Event:
                    return SymbolType.Event;
                case SymbolKind.Parameter:
                    return SymbolType.Parameter;
                case SymbolKind.TypeParameter:
                    return SymbolType.GenericParameter;
            }

            return SymbolType.Unknown;
        }

        public static ScriptModel.Symbols.INamespaceSymbol ResolveScriptNamespace(this Microsoft.CodeAnalysis.ISymbol symbol, IScriptModel scriptModel)
        {
            string namespaceName = symbol.ContainingNamespace.ResolveFullNamespaceName();
            return scriptModel.Namespaces.GetNamespace(namespaceName);
        }
    }
}
