using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    [ScriptName("IDictionary")]
    public interface IReadOnlyDictionary<TKey, TValue>
        : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        [DSharpScriptMemberName("keyExists")]
        bool ContainsKey(TKey key);

        [ScriptField]
        TValue this[TKey key] { get; }

        IEnumerable<TKey> Keys { get; }

        IEnumerable<TValue> Values { get; }
    }
}
