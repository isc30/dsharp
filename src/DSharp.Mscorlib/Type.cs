using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// The Type data type which is mapped to the Function type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed partial class Type
    {
        [ScriptName("$base")]
        [ScriptField]
        public extern Type BaseType { get; }

        // This has been changed from Name to FullName, we need to adjust the usage
        public extern string FullName { get; }

        [DSharpScriptMemberName("type")]
        public extern static Type GetType(string typeName);

        [DSharpScriptMemberName("canAssign")]
        public extern bool IsAssignableFrom(Type type);

        [DSharpScriptMemberName("isClass")]
        public extern static bool IsClass(Type type);

        [DSharpScriptMemberName("isInterface")]
        public extern static bool IsInterface(Type type);

        [DSharpScriptMemberName("instanceOf")]
        public extern bool IsInstanceOfType(object instance);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public extern static Type GetTypeFromHandle(RuntimeTypeHandle typeHandle);
    }
}
