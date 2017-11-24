using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    public interface IReadOnlyDictionary<TKey, TValue> : IReadonlyCollection<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; }

        IEnumerable<TKey> Keys { get; }

        IEnumerable<TValue> Values { get; }

        bool ContainsKey(TKey key);
    }
}