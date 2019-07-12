using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Collections.Generic
{
    public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        [ScriptField]
        TValue this[TKey key]
        {
            get;
            set;
        }

        ICollection<TKey> Keys { get; }

        ICollection<TValue> Values { get; }

        [DSharpScriptMemberName("keyExists")]
        bool ContainsKey(TKey key);

        void Add(TKey key, TValue value);

        bool Remove(TKey key);
    }
}
