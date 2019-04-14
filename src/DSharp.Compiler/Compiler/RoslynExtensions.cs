using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSharp.Compiler
{
    public static class RoslynExtensions
    {
        public static string ResolveFullNamespaceName(this Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol)
        {
            List<string> partialNames = new List<string>()
            {
                namespaceSymbol.Name
            };

            while (namespaceSymbol.ContainingNamespace != null)
            {
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
                if (string.IsNullOrEmpty(namespaceSymbol.Name))
                {
                    break;
                }

                partialNames.Add(namespaceSymbol.Name);
            }

            return string.Join(".", partialNames.Reverse<string>());
        }

        public static string ResolveName(this NameSyntax nameSyntax)
        {
            if (nameSyntax is IdentifierNameSyntax identifier)
            {
                return identifier.Identifier.ValueText;
            }
            else if (nameSyntax is QualifiedNameSyntax qualifiedName)
            {
                return ResolveName(qualifiedName.Left) + qualifiedName.DotToken.ValueText + ResolveName(qualifiedName.Right);
            }
            else if (nameSyntax is SimpleNameSyntax simpleName)
            {
                return simpleName.Identifier.ValueText;
            }

            return string.Empty;
        }

        public static IEnumerable<string> GetAllUsings(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.ResolveTypeUsings(us =>
            {
                return us.Where(item => item.Alias == null)
                    .Select(item => item.Name.ResolveName());
            });
        }

        public static IEnumerable<KeyValuePair<string, string>> GetAllAliases(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.ResolveTypeUsings(us =>
            {
                return us.Where(item => item.Alias != null).Select(item =>
                {
                    string alias = item.Name.ResolveName();
                    string typeName = item.Alias.Name.ResolveName();
                    return new KeyValuePair<string, string>(alias, typeName);
                });
            });
        }

        private static IEnumerable<T> ResolveTypeUsings<T>(
            this TypeDeclarationSyntax typeDeclarationSyntax,
            Func<SyntaxList<UsingDirectiveSyntax>, IEnumerable<T>> resolver)
        {
            if (typeDeclarationSyntax == null)
            {
                return Enumerable.Empty<T>();
            }

            List<T> allResolved = new List<T>();
            var currentNode = typeDeclarationSyntax.Parent;

            while (currentNode != null)
            {
                if (currentNode is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    var resolved = resolver.Invoke(namespaceDeclaration.Usings);
                    allResolved.AddRange(resolved);
                }
                else if (currentNode is CompilationUnitSyntax compilationUnitSyntax)
                {
                    var resolved = resolver.Invoke(compilationUnitSyntax.Usings);
                    allResolved.AddRange(resolved);
                }
                else
                {
                    Console.Error.WriteLine($"Unknown parent of type node '{currentNode.Kind()}' while resolving usings");
                }

                currentNode = currentNode.Parent;
            }

            return allResolved;
        }
    }
}
