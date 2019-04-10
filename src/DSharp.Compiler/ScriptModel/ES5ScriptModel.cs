using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using DSharp.Compiler.CodeModel;
using DSharp.Compiler.CodeModel.Names;
using DSharp.Compiler.CodeModel.Tokens;
using DSharp.Compiler.CodeModel.Types;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.ScriptModel
{
    public interface IScriptModel : IScriptSymbolTable
    {
        INamespaceSymbolCollection Namespaces { get; }

        ISymbolResolver SymbolResolver { get; }

        ScriptMetadata ScriptMetadata { get; set; }

        IEnumerable<ScriptReference> Dependencies { get; }

        IDocumenationResolver Documenation { get; }

        ISourceResourceProvider Resources { get; }

        void AddDependency(ScriptReference dependency);

        ScriptReference GetDependency(string name, out bool newReference);
    }

    //TODO: Implement the IOC Container and replace 
    public class ES5ScriptModel : IScriptModel
    {
        private readonly List<ScriptReference> dependencies = new List<ScriptReference>();
        private readonly Dictionary<string, ScriptReference> dependencySet = new Dictionary<string, ScriptReference>();

        public INamespaceSymbolCollection Namespaces { get; }

        public ScriptMetadata ScriptMetadata { get; set; }

        public IEnumerable<ISymbol> Symbols => Namespaces;

        public IEnumerable<ScriptReference> Dependencies => dependencies;

        public ISymbolResolver SymbolResolver { get; }

        public IDocumenationResolver Documenation { get; }

        public ISourceResourceProvider Resources { get; }

        public ES5ScriptModel()
        {
            Namespaces = new NamespaceSymbolCollection(this);
            SymbolResolver = new SymbolResolver(this);
            Documenation = new XmlDocumentationResolver();
            Resources = new SourceResources();
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

        public void AddDependency(ScriptReference dependency)
        {
            if (dependencySet.TryGetValue(dependency.Name, out ScriptReference existingDependency))
            {
                // The dependency already exists ... copy over identifier
                // from the new one to the existing one.

                // This is to support the scenario where a dependency got defined
                // by virtue of the app using a [ScriptReference] to specify path/delayLoad
                // semnatics, and we're finding the imported dependency later on
                // such as when a type with a dependency is referred in the code.

                if (existingDependency.HasIdentifier == false &&
                    dependency.HasIdentifier)
                {
                    existingDependency.Identifier = dependency.Identifier;
                }
            }
            else
            {
                dependencies.Add(dependency);
                dependencySet[dependency.Name] = dependency;
            }
        }

        public ScriptReference GetDependency(string name, out bool newReference)
        {
            newReference = false;

            if (dependencySet.TryGetValue(name, out ScriptReference reference) == false)
            {
                reference = new ScriptReference(name, null);
                newReference = true;
                AddDependency(reference);
            }

            return reference;
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
            this.root = root;

            Global = new NamespaceSymbol(string.Empty, root);
            Global.SetTransformedName(string.Empty);

            System = new NamespaceSymbol(SYSTEM_NAMESPACE, root);

            namespaces.Add(Global);
            namespaces.Add(System);
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

    public class SymbolResolver : ISymbolResolver
    {
        private const string ARRAY_SYMBOL_NAME = "Array";

        private static readonly Dictionary<IntrinsicType, (string name, string @namespace)> intrinsicTypeMap
             = new Dictionary<IntrinsicType, (string name, string @namespace)>
             {
                 [IntrinsicType.Object] = ("Object", null),
                 [IntrinsicType.Boolean] = ("Boolean", null),
                 [IntrinsicType.String] = ("String", null),
                 [IntrinsicType.Integer] = ("Int32", null),
                 [IntrinsicType.UnsignedInteger] = ("UInt32", null),
                 [IntrinsicType.Long] = ("Int64", null),
                 [IntrinsicType.UnsignedLong] = ("UInt64", null),
                 [IntrinsicType.Short] = ("Int16", null),
                 [IntrinsicType.UnsignedShort] = ("UInt16", null),
                 [IntrinsicType.Byte] = ("Byte", null),
                 [IntrinsicType.SignedByte] = ("SByte", null),
                 [IntrinsicType.Single] = ("Single", null),
                 [IntrinsicType.Date] = ("Date", null),
                 [IntrinsicType.Decimal] = ("Decimal", null),
                 [IntrinsicType.Double] = ("Double", null),
                 [IntrinsicType.Delegate] = ("Delegate", null),
                 [IntrinsicType.Function] = ("Function", null),
                 [IntrinsicType.Void] = ("Void", null),
                 [IntrinsicType.Array] = ("Array", null),
                 [IntrinsicType.Dictionary] = ("Dictionary", "System.Collections"),
                 [IntrinsicType.GenericList] = ("List`1", "System.Collections.Generic"),
                 [IntrinsicType.GenericDictionary] = ("Dictionary`2", "System.Collections.Generic"),
                 [IntrinsicType.Type] = ("Type", null),
                 [IntrinsicType.Enumerator] = ("IEnumerator", "System.Collections"),
                 [IntrinsicType.Enum] = ("Enum", null),
                 [IntrinsicType.Exception] = ("Exception", null),
                 [IntrinsicType.Script] = ("Script", null),
                 [IntrinsicType.Number] = ("Number", null),
                 [IntrinsicType.Arguments] = ("Arguments", null),
                 [IntrinsicType.Nullable] = ("Nullable`1", null)
             };

        private readonly IScriptModel root;
        private Dictionary<string, ITypeSymbol> arrayTypeTable = new Dictionary<string, ITypeSymbol>();
        private Dictionary<string, ITypeSymbol> genericTypeTable = new Dictionary<string, ITypeSymbol>();

        private INamespaceSymbol SystemNamespace => root.Namespaces.System;

        public SymbolResolver(IScriptModel root)
        {
            this.root = root;
        }

        public ITypeSymbol CreateArrayTypeSymbol(ITypeSymbol itemTypeSymbol)
        {
            if (arrayTypeTable.TryGetValue(itemTypeSymbol?.FullName, out var symbol))
            {
                return symbol;
            }

            ITypeSymbol specificArrayTypeSymbol = CreateArrayTypeCore(itemTypeSymbol);
            arrayTypeTable.Add(itemTypeSymbol.FullName, specificArrayTypeSymbol);

            return specificArrayTypeSymbol;
        }

        public ITypeSymbol CreateGenericTypeSymbol(ITypeSymbol templateType, IList<ITypeSymbol> typeArguments)
        {
            foreach (TypeSymbol typeSymbol in typeArguments)
            {
                if (typeSymbol.Type == SymbolType.GenericParameter)
                {
                    return templateType;
                }
            }

            StringBuilder keyBuilder = new StringBuilder(templateType.FullName);

            foreach (TypeSymbol typeSymbol in typeArguments)
            {
                keyBuilder.Append("+");
                keyBuilder.Append(typeSymbol.FullName);
            }

            string key = keyBuilder.ToString();

            if (genericTypeTable.TryGetValue(key, out var genericTypeSymbol))
            {
                return genericTypeSymbol;
            }

            // Prepopulate with a placeholder ... if a generic type's member refers to its
            // parent type it will use the type being created when the return value is null.
            genericTypeTable.Add(key, null);

            ITypeSymbol instanceTypeSymbol = CreateGenericTypeCore(templateType, typeArguments);
            genericTypeTable[key] = instanceTypeSymbol;

            return instanceTypeSymbol;
        }

        public ITypeSymbol ResolveIntrinsicType(IntrinsicType type)
        {
            if (!intrinsicTypeMap.TryGetValue(type, out var result))
            {
                Debug.Fail("Unmapped intrinsic type " + type);
                return null;
            }

            string typeName = result.name;

            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            INamespaceSymbol namespaceSymbol = root.Namespaces.GetNamespace(result.@namespace) ?? root.Namespaces.System;
            Debug.Assert(namespaceSymbol != null);
            TypeSymbol typeSymbol = (TypeSymbol)namespaceSymbol?.FindSymbol(typeName, null, SymbolFilter.Types);
            Debug.Assert(typeSymbol != null);

            return typeSymbol;
        }

        public ITypeSymbol ResolveType(ParseNode node, IScriptSymbolTable symbolTable, ISymbol contextSymbol)
        {
            if (node is IntrinsicTypeNode intrinsicTypeNode)
            {
                IntrinsicType intrinsicType = ResolveIntrinsticNodeType(intrinsicTypeNode);
                ITypeSymbol typeSymbol = ResolveIntrinsicType(intrinsicType);

                if (intrinsicTypeNode.IsNullable)
                {
                    ITypeSymbol nullableType = ResolveIntrinsicType(IntrinsicType.Nullable);
                    typeSymbol = CreateGenericTypeSymbol(nullableType, new List<ITypeSymbol> { typeSymbol });
                }

                return typeSymbol;
            }
            else if (node is ArrayTypeNode arrayTypeNode)
            {
                ITypeSymbol itemTypeSymbol = ResolveType(arrayTypeNode.BaseType, symbolTable, contextSymbol);
                Debug.Assert(itemTypeSymbol != null);

                return CreateArrayTypeSymbol(itemTypeSymbol);
            }
            else if (node is GenericNameNode genericNameNode)
            {
                string genericTypeName = genericNameNode.Name + "`" + genericNameNode.TypeArguments.Count;
                TypeSymbol templateType =
                    (TypeSymbol)symbolTable.FindSymbol(genericTypeName, contextSymbol, SymbolFilter.Types);

                List<ITypeSymbol> typeArguments = new List<ITypeSymbol>();

                foreach (ParseNode argNode in genericNameNode.TypeArguments)
                {
                    ITypeSymbol argType = ResolveType(argNode, symbolTable, contextSymbol);
                    typeArguments.Add(argType);
                }

                ITypeSymbol resolvedSymbol = CreateGenericTypeSymbol(templateType, typeArguments);
                Debug.Assert(resolvedSymbol != null);

                return resolvedSymbol;
            }
            else if (node is NameNode nameNode)
            {
                return (TypeSymbol)symbolTable.FindSymbol(nameNode.Name, contextSymbol, SymbolFilter.Types);
            }

            return null;
        }

        private ITypeSymbol CreateArrayTypeCore(ITypeSymbol itemTypeSymbol)
        {
            TypeSymbol arrayTypeSymbol = (TypeSymbol)SystemNamespace.FindSymbol(ARRAY_SYMBOL_NAME, null, SymbolFilter.Types);
            Debug.Assert(arrayTypeSymbol != null);

            TypeSymbol specificArrayTypeSymbol = new ClassSymbol(ARRAY_SYMBOL_NAME, SystemNamespace);
            foreach (MemberSymbol memberSymbol in arrayTypeSymbol.Members)
            {
                specificArrayTypeSymbol.AddMember(memberSymbol);
            }

            IndexerSymbol indexerSymbol = new IndexerSymbol(specificArrayTypeSymbol, itemTypeSymbol, MemberVisibility.Public);
            indexerSymbol.SetScriptIndexer();
            specificArrayTypeSymbol.AddMember(indexerSymbol);
            specificArrayTypeSymbol.SetIgnoreNamespace();
            specificArrayTypeSymbol.SetArray();

            return specificArrayTypeSymbol;
        }

        private ITypeSymbol CreateGenericTypeCore(ITypeSymbol templateType, IList<ITypeSymbol> typeArguments)
        {
            if (templateType.Type == SymbolType.Class)
            {
                ClassSymbol genericClass = (ClassSymbol)templateType;
                ClassSymbol instanceClass = new ClassSymbol(genericClass.Name, (NamespaceSymbol)genericClass.Parent);
                instanceClass.SetInheritance(genericClass.BaseClass, genericClass.Interfaces);
                instanceClass.SetImported(genericClass.Dependency);

                if (genericClass.IgnoreNamespace)
                {
                    instanceClass.SetIgnoreNamespace();
                }

                instanceClass.ScriptNamespace = genericClass.ScriptNamespace;

                if (genericClass.IsTransformed)
                {
                    instanceClass.SetTransformedName(genericClass.GeneratedName);
                }
                else if (genericClass.IsTransformAllowed == false)
                {
                    instanceClass.DisableNameTransformation();
                }

                if (genericClass.IsArray)
                {
                    instanceClass.SetArray();
                }

                instanceClass.AddGenericParameters(genericClass.GenericParameters);
                instanceClass.AddGenericArguments(genericClass, typeArguments);

                CreateGenericTypeMembers(genericClass, instanceClass, typeArguments);

                return instanceClass;
            }

            if (templateType.Type == SymbolType.Interface)
            {
                InterfaceSymbol genericInterface = (InterfaceSymbol)templateType;
                InterfaceSymbol instanceInterface =
                    new InterfaceSymbol(genericInterface.Name, (NamespaceSymbol)genericInterface.Parent);

                instanceInterface.SetImported(genericInterface.Dependency);

                if (genericInterface.IgnoreNamespace)
                {
                    instanceInterface.SetIgnoreNamespace();
                }

                if (genericInterface.IsTransformed)
                {
                    instanceInterface.SetTransformedName(genericInterface.GeneratedName);
                }
                else if (genericInterface.IsTransformAllowed == false)
                {
                    instanceInterface.DisableNameTransformation();
                }

                instanceInterface.AddGenericParameters(genericInterface.GenericParameters);
                instanceInterface.AddGenericArguments(genericInterface, typeArguments);

                CreateGenericTypeMembers(genericInterface, instanceInterface, typeArguments);

                return instanceInterface;
            }

            if (templateType.Type == SymbolType.Delegate)
            {
                DelegateSymbol genericDelegate = (DelegateSymbol)templateType;
                DelegateSymbol instanceDelegate =
                    new DelegateSymbol(genericDelegate.Name, (NamespaceSymbol)genericDelegate.Parent);

                instanceDelegate.AddGenericParameters(genericDelegate.GenericParameters);
                instanceDelegate.AddGenericArguments(genericDelegate, typeArguments);

                CreateGenericTypeMembers(genericDelegate, instanceDelegate, typeArguments);

                return instanceDelegate;
            }

            return null;
        }

        private IMemberSymbol CreateGenericMember(IMemberSymbol templateMember, IList<ITypeSymbol> typeArguments)
        {
            ITypeSymbol parentType = (ITypeSymbol)templateMember.Parent;
            ITypeSymbol instanceAssociatedType;

            if (templateMember.AssociatedType.Type == SymbolType.GenericParameter)
            {
                GenericParameterSymbol genericParameter = (GenericParameterSymbol)templateMember.AssociatedType;
                instanceAssociatedType = typeArguments[genericParameter.Index];
            }
            else
            {
                instanceAssociatedType = typeArguments[0];
            }

            if (templateMember.Type == SymbolType.Indexer)
            {
                IndexerSymbol templateIndexer = (IndexerSymbol)templateMember;
                IndexerSymbol instanceIndexer = new IndexerSymbol(parentType, instanceAssociatedType);

                if (templateIndexer.UseScriptIndexer)
                {
                    instanceIndexer.SetScriptIndexer();
                }

                instanceIndexer.SetVisibility(templateIndexer.Visibility);

                return instanceIndexer;
            }

            if (templateMember.Type == SymbolType.Property)
            {
                PropertySymbol templateProperty = (PropertySymbol)templateMember;
                PropertySymbol instanceProperty =
                    new PropertySymbol(templateProperty.Name, parentType, instanceAssociatedType);

                if (templateProperty.IsTransformed)
                {
                    instanceProperty.SetTransformedName(templateProperty.GeneratedName);
                }

                instanceProperty.SetNameCasing(templateProperty.IsCasePreserved);
                instanceProperty.SetVisibility(templateProperty.Visibility);

                return instanceProperty;
            }

            if (templateMember.Type == SymbolType.Field)
            {
                FieldSymbol templateField = (FieldSymbol)templateMember;
                FieldSymbol instanceField = new FieldSymbol(templateField.Name, parentType, instanceAssociatedType);

                if (templateField.IsTransformed)
                {
                    instanceField.SetTransformedName(templateField.GeneratedName);
                }

                instanceField.SetNameCasing(templateField.IsCasePreserved);
                instanceField.SetVisibility(templateField.Visibility);

                return instanceField;
            }

            if (templateMember.Type == SymbolType.Method)
            {
                MethodSymbol templateMethod = (MethodSymbol)templateMember;
                MethodSymbol instanceMethod = new MethodSymbol(templateMethod.Name, parentType, instanceAssociatedType);

                if (templateMethod.IsAliased)
                {
                    instanceMethod.SetTransformName(templateMethod.TransformName);
                }
                else if (templateMethod.IsTransformed)
                {
                    instanceMethod.SetTransformedName(templateMethod.GeneratedName);
                }

                if (templateMethod.SkipGeneration)
                {
                    instanceMethod.SetSkipGeneration();
                }

                if (templateMethod.InterfaceMember != null)
                {
                    instanceMethod.SetInterfaceMember(templateMethod.InterfaceMember);
                }

                instanceMethod.SetNameCasing(templateMethod.IsCasePreserved);
                instanceMethod.SetVisibility(templateMethod.Visibility);

                return instanceMethod;
            }

            Debug.Fail("Unexpected generic member '" + templateMember.Name + " on type '" +
                       ((TypeSymbol)templateMember.Parent).FullName + "'.");

            return null;
        }

        private void CreateGenericTypeMembers(ITypeSymbol templateType, ITypeSymbol instanceType, IList<ITypeSymbol> typeArguments)
        {
            foreach (MemberSymbol memberSymbol in templateType.Members)
            {
                if (memberSymbol.AssociatedType.Type == SymbolType.GenericParameter &&
                    ((GenericParameterSymbol)memberSymbol.AssociatedType).IsTypeParameter)
                {
                    IMemberSymbol instanceMemberSymbol = CreateGenericMember(memberSymbol, typeArguments);
                    instanceType.AddMember(instanceMemberSymbol);
                }
                else if (memberSymbol.AssociatedType.IsGeneric &&
                         memberSymbol.AssociatedType.GenericArguments == null &&
                         memberSymbol.AssociatedType.GenericParameters.Count == typeArguments.Count)
                {
                    ITypeSymbol genericType = CreateGenericTypeSymbol(memberSymbol.AssociatedType, typeArguments);

                    if (genericType == null)
                    {
                        genericType = instanceType;
                    }

                    List<ITypeSymbol> memberTypeArgs = new List<ITypeSymbol> { genericType };

                    IMemberSymbol instanceMemberSymbol = CreateGenericMember(memberSymbol, memberTypeArgs);
                    instanceType.AddMember(instanceMemberSymbol);
                }
                else
                {
                    instanceType.AddMember(memberSymbol);
                }
            }

            IndexerSymbol indexer = null;

            if (templateType.Type == SymbolType.Class)
            {
                indexer = ((ClassSymbol)templateType).Indexer;
            }
            else if (templateType.Type == SymbolType.Interface)
            {
                indexer = ((InterfaceSymbol)templateType).Indexer;
            }

            if (indexer != null)
            {
                if (indexer.AssociatedType.Type == SymbolType.GenericParameter)
                {
                    IMemberSymbol instanceIndexer = CreateGenericMember(indexer, typeArguments);
                    instanceType.AddMember(instanceIndexer);
                }
                else if (indexer.AssociatedType.IsGeneric &&
                         indexer.AssociatedType.GenericArguments == null &&
                         indexer.AssociatedType.GenericParameters.Count == typeArguments.Count)
                {
                    ITypeSymbol genericType = CreateGenericTypeSymbol(indexer.AssociatedType, typeArguments);

                    if (genericType == null)
                    {
                        genericType = instanceType;
                    }

                    List<ITypeSymbol> memberTypeArgs = new List<ITypeSymbol> { genericType };

                    IMemberSymbol instanceMemberSymbol = CreateGenericMember(indexer, memberTypeArgs);
                    instanceType.AddMember(instanceMemberSymbol);
                }
                else
                {
                    instanceType.AddMember(indexer);
                }
            }
        }

        private IntrinsicType ResolveIntrinsticNodeType(IntrinsicTypeNode intrinsicTypeNode)
        {
            switch (intrinsicTypeNode.Type)
            {
                case TokenType.Object:
                    return IntrinsicType.Object;
                case TokenType.Bool:
                    return IntrinsicType.Boolean;
                case TokenType.String:
                case TokenType.Char:
                    return IntrinsicType.String;
                case TokenType.UInt:
                    return IntrinsicType.UnsignedInteger;
                case TokenType.Long:
                    return IntrinsicType.Long;
                case TokenType.ULong:
                    return IntrinsicType.UnsignedLong;
                case TokenType.Short:
                    return IntrinsicType.Short;
                case TokenType.UShort:
                    return IntrinsicType.UnsignedShort;
                case TokenType.Byte:
                    return IntrinsicType.Byte;
                case TokenType.SByte:
                    return IntrinsicType.SignedByte;
                case TokenType.Float:
                    return IntrinsicType.Single;
                case TokenType.Decimal:
                    return IntrinsicType.Decimal;
                case TokenType.Double:
                    return IntrinsicType.Double;
                case TokenType.Delegate:
                    return IntrinsicType.Delegate;
                case TokenType.Void:
                    return IntrinsicType.Void;
                case TokenType.Int:
                default:
                    return IntrinsicType.Integer;
            }
        }
    }

    public interface ISymbolResolver
    {
        ITypeSymbol CreateArrayTypeSymbol(ITypeSymbol itemTypeSymbol);

        ITypeSymbol CreateGenericTypeSymbol(ITypeSymbol templateType, IList<ITypeSymbol> typeArguments);

        ITypeSymbol ResolveIntrinsicType(IntrinsicType type);

        ITypeSymbol ResolveType(ParseNode node, IScriptSymbolTable symbolTable, ISymbol contextSymbol);
    }

    public interface IDocumenationResolver
    {
        string GetSummaryDocumentation(string documentationId);

        string GetParameterDocumentation(string id, string paramName);

        void LoadDocument(XmlDocument xmlDocument);
    }

    public class XmlDocumentationResolver : IDocumenationResolver
    {
        private XmlDocument documentationXml = null;

        public string GetParameterDocumentation(string id, string paramName)
        {
            if (documentationXml != null)
            {
                XmlNode paramNode = documentationXml.SelectSingleNode(
                    string.Format("//doc/members/member[@name='{0}']/param[@name='{1}']", id, paramName));

                if (paramNode != null)
                {
                    return paramNode.InnerXml;
                }
            }

            return string.Empty;
        }

        public string GetSummaryDocumentation(string documentationId)
        {
            if (documentationXml != null)
            {
                XmlNode summaryNode = documentationXml.SelectSingleNode(
                    string.Format("//doc/members/member[@name='{0}']/summary", documentationId));

                if (summaryNode != null)
                {
                    return summaryNode.InnerXml;
                }
            }

            return string.Empty;
        }

        public void LoadDocument(XmlDocument xmlDocument)
        {
            if (xmlDocument == null)
            {
                return;
            }

            documentationXml = xmlDocument;
        }
    }

    public static class ScriptModelExtensions
    {
        public static bool IsSymbol(this IScriptModel scriptModel, ITypeSymbol typeSymbol, string symbolName)
        {
            return scriptModel?.FindSymbol(symbolName, null, SymbolFilter.Types) == typeSymbol;
        }
    }

    public interface ISourceResourceProvider
    {
        bool HasResources { get; }

        Dictionary<string, ResXItem> GetResources(string name);
    }

    public class SourceResources : ISourceResourceProvider
    {
        private readonly Dictionary<string, Dictionary<string, ResXItem>> resources
            = new Dictionary<string, Dictionary<string, ResXItem>>(StringComparer.OrdinalIgnoreCase);

        public bool HasResources => resources.Any();

        public Dictionary<string, ResXItem> GetResources(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (resources.TryGetValue(name, out Dictionary<string, ResXItem> resourceTable))
            {
                return resourceTable;
            }

            resourceTable = new Dictionary<string, ResXItem>(StringComparer.Ordinal);
            resources[name] = resourceTable;

            return resourceTable;
        }
    }
}
