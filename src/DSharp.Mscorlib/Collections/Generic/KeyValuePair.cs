using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Object")]
    public sealed class KeyValuePair<TKey, TValue>
    {
        internal KeyValuePair() { }

        [ScriptField]
        public extern TKey Key { get; }

        [ScriptField]
        public extern TValue Value { get; }
    }
}
