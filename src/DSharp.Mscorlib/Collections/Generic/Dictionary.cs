using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>
    /// The Dictionary data type which is mapped to the Object type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Object")]
    // In CLR this is an interface (IDictionary) therefore no ctors are defined.
    public sealed partial class Dictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> // In CLR is inheriting from ICollection<KeyValuePair<TKey, TValue>> where TKey : notnull.

    {
        public Dictionary() { }

        public Dictionary(params object[] nameValuePairs) { }

        [Obsolete("This is only for use by the c# compiler, and cannot be used for generating script.", /* error */ true)]
        public extern Dictionary(int count);

        public extern IReadOnlyCollection<TKey> Keys { get; } // Return type should be ICollection<TKey>

        // We are missing the Values property -> ICollection<TValue> Values { get; }

        [ScriptField]
        public extern TValue this[TKey key] { get; set; }

        [Obsolete("This is only for use by the c# compiler, and cannot be used for generating script.", /* error */ true)]
        public extern void Add(TKey key, TValue value);

        [DSharpScriptMemberName("clearKeys")]
        public extern void Clear();

        [DSharpScriptMemberName("keyExists")]
        public extern bool ContainsKey(TKey key);

        // Modified to return bool. Check implementation.
        public extern void Remove(TKey key);

        public extern static implicit operator Dictionary(Dictionary<TKey, TValue> dictionary);

        public extern static implicit operator Dictionary<TKey, TValue>(Dictionary dictionary);
    }
}
