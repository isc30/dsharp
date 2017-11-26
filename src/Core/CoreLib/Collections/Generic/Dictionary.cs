// Dictionary.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>
    /// The Dictionary data type which is mapped to the Object type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Dictionary")]
    public sealed class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public extern Dictionary(IDictionary<TKey, TValue> dictionary);

        public extern int Count { get; }

        public extern ICollection<TKey> Keys { get; }

        public extern ICollection<TValue> Values { get; }

        extern IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { get; }

        extern IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { get; }

        [ScriptField]
        [ScriptName("_instance")]
        public extern TValue this[TKey key]
        {
            get;
            set;
        }

        public extern void Add(TKey key, TValue value);

        public extern void Clear();

        public extern bool ContainsKey(TKey key);

        public extern IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        public extern bool Remove(TKey key);

        public extern bool TryGetValue(TKey key, out TValue value);

        extern IEnumerator IEnumerable.GetEnumerator();

        extern void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item);

        extern bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item);

        extern bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item);
    }
}
