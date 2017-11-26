// Object.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Runtime.CompilerServices;

namespace System {

    /// <summary>
    /// Equivalent to the Object type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class Object {

        [ScriptName("name")]
        internal const string NAME_DEFINITION = "Object";

        /// <summary>
        /// Retrieves the type associated with an object instance.
        /// </summary>
        /// <returns>The type of the object.</returns>
        [ScriptAlias("ss.typeOf")]
        public extern Type GetType();

        /// <summary>
        /// Converts an object to its string representation.
        /// </summary>
        /// <returns>The string representation of the object.</returns>
        public extern virtual string ToString();

        [ScriptAlias("ss.getHashCode")]
        public extern virtual int GetHashCode();
    }
}
