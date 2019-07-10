using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>
    /// The Dictionary data type which is mapped to the Object type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Object")]
    public sealed class Dictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public Dictionary() { }

        public Dictionary(params object[] nameValuePairs) { }

        [Obsolete("This is only for use by the c# compiler, and cannot be used for generating script.", /* error */ true)]
        public extern Dictionary(int count);

        public extern int Count { get; }

        public extern IReadOnlyCollection<TKey> Keys { get; }

        [ScriptField]
        public extern TValue this[TKey key] { get; set; }

        [Obsolete("This is only for use by the c# compiler, and cannot be used for generating script.", /* error */ true)]
        public extern void Add(TKey key, TValue value);

        [DSharpScriptMemberName("clearKeys")]
        public extern void Clear();

        [DSharpScriptMemberName("keyExists")]
        public extern bool ContainsKey(TKey key);

        public extern static Dictionary<TKey, TValue> GetDictionary(object o);

        public extern IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        extern IEnumerator IEnumerable.GetEnumerator();

        public extern void Remove(TKey key);

        [Obsolete("This is only for use by the c# compiler, and cannot be used for generating script.", /* error */ true)]
        public extern bool TryGetValue(TKey key, out TValue value);

        public extern static implicit operator Dictionary(Dictionary<TKey, TValue> dictionary);

        public extern static implicit operator Dictionary<TKey, TValue>(Dictionary dictionary);
    }
}
