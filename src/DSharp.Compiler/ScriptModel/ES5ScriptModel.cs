using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.ScriptModel
{
    public class ES5ScriptModel : IScriptModel
    {
        public INamespaceSymbolCollection Namespaces { get; }

        public string ScriptName { get; set; }

        public IEnumerable<ISymbol> Symbols => Namespaces;

        public ES5ScriptModel()
        {
            Namespaces = new NamespaceSymbolCollection(this);
        }

        public ISymbol FindSymbol(string name, ISymbol context, SymbolFilter filter)
        {
            if ((filter & SymbolFilter.Types) == 0)
            {
                return null;
            }

            ISymbol symbol = null;

            if (name.IndexOf('.') > 0)
            {
                int nameIndex = name.LastIndexOf('.') + 1;
                Debug.Assert(nameIndex < name.Length);

                string namespaceName = name.Substring(0, nameIndex - 1);
                name = name.Substring(nameIndex);

                if (Namespaces.TryGetNamespace(namespaceName, out INamespaceSymbol namespaceSymbol))
                {
                    symbol = ((IScriptSymbolTable)namespaceSymbol).FindSymbol(name, /* context */ null, SymbolFilter.Types);
                }
            }
            else
            {
                Debug.Assert(context != null);

                TypeSymbol typeSymbol = context as TypeSymbol;

                if (typeSymbol == null)
                {
                    ISymbol parentSymbol = context.Parent;

                    while (parentSymbol != null)
                    {
                        typeSymbol = parentSymbol as TypeSymbol;

                        if (typeSymbol != null)
                        {
                            break;
                        }

                        parentSymbol = parentSymbol.Parent;
                    }
                }

                Debug.Assert(typeSymbol != null);

                if (typeSymbol == null)
                {
                    return null;
                }

                bool systemNamespaceChecked = false;

                NamespaceSymbol containerNamespace = (NamespaceSymbol)typeSymbol.Parent;
                Debug.Assert(containerNamespace != null);

                symbol = ((IScriptSymbolTable)containerNamespace).FindSymbol(name, /* context */ null, SymbolFilter.Types);

                if (containerNamespace == Namespaces.System)
                {
                    systemNamespaceChecked = true;
                }

                if (symbol == null)
                {
                    if (typeSymbol.Aliases != null && typeSymbol.Aliases.ContainsKey(name))
                    {
                        string typeReference = typeSymbol.Aliases[name];
                        symbol = ((IScriptSymbolTable)this).FindSymbol(typeReference, /* context */ null,
                            SymbolFilter.Types);
                    }
                    else if (typeSymbol.Imports != null)
                    {
                        foreach (string importedNamespaceReference in typeSymbol.Imports)
                        {
                            if (!Namespaces.TryGetNamespace(importedNamespaceReference, out INamespaceSymbol importedNamespace) || importedNamespace == containerNamespace)
                            {
                                // Since we included all parent namespaces of the current type's
                                // namespace, we might run into a namespace that doesn't contain
                                // any defined types, i.e. doesn't exist.
                                continue;
                            }

                            symbol = ((IScriptSymbolTable)importedNamespace).FindSymbol(name, null, SymbolFilter.Types);

                            if (importedNamespace == Namespaces.System)
                            {
                                systemNamespaceChecked = true;
                            }

                            if (symbol != null)
                            {
                                break;
                            }
                        }
                    }
                }

                if (symbol == null && systemNamespaceChecked == false)
                {
                    symbol = ((IScriptSymbolTable)Namespaces.System).FindSymbol(name, /* context */ null, SymbolFilter.Types);
                }

                if (symbol == null)
                {
                    symbol = ((IScriptSymbolTable)Namespaces.Global).FindSymbol(name, /* context */ null, SymbolFilter.Types);
                }
            }

            return symbol;
        }
    }

    public interface INamespaceSymbolCollection : IEnumerable<INamespaceSymbol>
    {
        INamespaceSymbol System { get; }

        INamespaceSymbol Global { get; }

        /// <summary>
        /// Attempts to get the specified namedpace symbol. If none is found it will create it and return a new instance.
        /// </summary>
        /// <param name="namespaceName">The full name of the namespace</param>
        /// <returns></returns>
        INamespaceSymbol GetNamespace(string namespaceName);

        /// <summary>
        /// Attempts to get the specified namedpace symbol.
        /// </summary>
        /// <param name="namespaceName">The full name of the namespace.</param>
        /// <param name="namespaceSymbol">The retrieved symbol.</param>
        /// <returns></returns>
        bool TryGetNamespace(string namespaceName, out INamespaceSymbol namespaceSymbol);
    }

    public class NamespaceSymbolCollection : INamespaceSymbolCollection
    {
        public const string SYSTEM_NAMESPACE = "System";

        private readonly HashSet<INamespaceSymbol> namespaces = new HashSet<INamespaceSymbol>();
        private readonly IScriptModel root;

        public INamespaceSymbol System { get; }

        public INamespaceSymbol Global { get; }

        public NamespaceSymbolCollection(IScriptModel root)
        {
            Global = new NamespaceSymbol(string.Empty, root);
            Global.SetTransformedName(string.Empty);

            System = new NamespaceSymbol(SYSTEM_NAMESPACE, root);
            this.root = root;
        }

        public IEnumerator<INamespaceSymbol> GetEnumerator()
        {
            return namespaces.GetEnumerator();
        }

        public bool TryGetNamespace(string namespaceName, out INamespaceSymbol namespaceSymbol)
        {
            namespaceSymbol = null;

            var item = namespaces.FirstOrDefault(ns => ns.Name == namespaceName);
            if (item == null)
            {
                return false;
            }

            namespaceSymbol = item;
            return true;
        }

        public INamespaceSymbol GetNamespace(string namespaceName)
        {
            if (!TryGetNamespace(namespaceName, out var namespaceSymbol))
            {
                namespaceSymbol = CreateNamespace(namespaceName);
            }

            return namespaceSymbol;
        }

        private INamespaceSymbol CreateNamespace(string namespaceName)
        {
            if (namespaceName.IndexOf('.') > 0)
            {
                NamespaceSymbol namespaceSymbol = new NamespaceSymbol(namespaceName, root);
                namespaces.Add(namespaceSymbol);
                return namespaceSymbol;
            }
            // Split up the namespace into its individual parts, and then
            // create namespace symbols for each sub-namespace leading up
            // to the full specified namespace

            string[] namespaceParts = namespaceName.Split('.');

            for (int i = 0; i < namespaceParts.Length; i++)
            {
                string partialNamespace;

                if (i == 0)
                {
                    partialNamespace = namespaceParts[0];
                }
                else
                {
                    partialNamespace = string.Join(".", namespaceParts, 0, i + 1);
                }

                NamespaceSymbol namespaceSymbol = new NamespaceSymbol(partialNamespace, root);
                namespaces.Add(namespaceSymbol);
            }

            //This needs to be reworked, as it could potentially remake the symbol, and is not optimised.
            return GetNamespace(namespaceName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return namespaces.GetEnumerator();
        }
    }
}
