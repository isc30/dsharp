﻿using System;
using System.Collections.Generic;
using System.Linq;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.Extensions
{
    public static class TypeSymbolExtensions
    {
        internal static ICollection<InterfaceSymbol> GetInterfaces(this ITypeSymbol symbol)
        {
            if (symbol is ClassSymbol classSymbol)
            {
                return classSymbol.Interfaces ?? Array.Empty<InterfaceSymbol>();
            }

            if (symbol is InterfaceSymbol interfaceSymbol)
            {
                var interfaces = new List<InterfaceSymbol> { interfaceSymbol };

                if (interfaceSymbol.Interfaces != null)
                {
                    interfaces.AddRange(interfaceSymbol.Interfaces);
                }

                return interfaces;
            }

            return null;
        }

        internal static bool IsCollectionType(this ITypeSymbol symbol)
        {
            var interfaces = symbol.GetInterfaces();

            if (interfaces == null)
            {
                return false;
            }

            if (interfaces.Any(i => i.FullName.StartsWith("System.Collections") && i.FullGeneratedName == "ICollection"))
            {
                return true;
            }

            return false;
        }
    }
}
