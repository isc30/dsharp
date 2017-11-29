// Type.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Browser;

namespace System
{
    /// <summary>
    /// The Type data type which is mapped to the Function type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class Type
    {
        [ScriptName("$base")]
        [ScriptField]
        public extern Type BaseType { get; }

        public extern string Name { get; }
        

        [ScriptName("$namespace")]
        [ScriptField]
        public extern string Namespace
        {
            get;
        }

        [ScriptName("$fullName")]
        [ScriptField]
        public extern string FullName
        {
            get;
        }

        [ScriptName("$assembly")]
        [ScriptField]
        public extern Assembly Assembly
        {
            get;
		}

        [ScriptField]
        public extern JsDictionary Prototype { get; }

        [ScriptAlias("ss.type")]
        public extern static Type GetType(string typeName);

        [ScriptAlias("ss.canAssign")]
        public extern bool IsAssignableFrom(Type type);

        [ScriptAlias("ss.isClass")]
        public extern static bool IsClass(Type type);

        [ScriptAlias("ss.isInterface")]
        public extern static bool IsInterface(Type type);

        [ScriptAlias("ss.instanceOf")]
        public extern bool IsInstanceOfType(object instance);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public extern static Type GetTypeFromHandle(RuntimeTypeHandle typeHandle);
    }
}
