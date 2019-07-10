using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    //
    [ScriptImport]
    [ScriptName("ICollection")]
    public interface ICollection<T> : IEnumerable<T>
    {
        [ScriptField]
        [ScriptName("length")]
        int Count { get; }

        [ScriptName("push")]
        void Add(T item);

        [ScriptName("push")]
        void AddRange(params T[] items);

        void Clear();

        bool Contains(T item);

        [DSharpScriptMemberName("remove")]
        bool Remove(T item);
    }
}
