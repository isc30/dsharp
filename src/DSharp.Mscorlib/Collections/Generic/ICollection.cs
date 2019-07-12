using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    [ScriptName("ICollection")]
    public interface ICollection<T> : IEnumerable<T>, IEnumerable
    {
        [ScriptField]
        [ScriptName("length")]
        int Count { get; }

        [ScriptName("push")]
        void Add(T item);

        void Clear();

        bool Contains(T item);

        [DSharpScriptMemberName("remove")]
        bool Remove(T item);
    }
}
