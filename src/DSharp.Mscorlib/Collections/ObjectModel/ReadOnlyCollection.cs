using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Collections.ObjectModel
{
    //
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Array")]
    public class ReadOnlyCollection<T> : IList<T>, IList, IReadOnlyList<T>
    {
        [ScriptField]
        [ScriptName("length")]
        public extern int Count { get; }

        [ScriptField]
        extern T IList<T>.this[int index] { get; set; }

        [ScriptField]
        extern T IReadOnlyList<T>.this[int index] { get; }

        [ScriptField]
        public extern object this[int index] { get; set; }

        [ScriptName("push")]
        public extern int Add(object value);

        [ScriptName("push")]
        public extern void Add(T item);

        [ScriptName("push")]
        public extern void AddRange(params object[] items);

        [ScriptName("push")]
        public extern void AddRange(params T[] items);

        public extern bool Contains(object value);

        public extern bool Contains(T item);

        public extern void Clear();

        public extern int IndexOf(object value);

        public extern int IndexOf(T item);

        [DSharpScriptMemberName("remove")]
        public extern void Remove(object value);

        [DSharpScriptMemberName("remove")]
        public extern bool Remove(T item);

        public extern void RemoveAt(int index);

        public extern IEnumerator GetEnumerator();

        extern IEnumerator<T> IEnumerable<T>.GetEnumerator();

        public extern void Insert(int index, T item);
    }
}
