using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    // NOTE: Keep in sync with ArrayList and Array

    /// <summary>
    /// Equivalent to the Array type in Javascript.
    /// </summary>
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Array")]
    public sealed partial class List<T> : IList<T>, IList, IReadOnlyList<T>
    {
        public List() { }

        public List(int capacity) { }

        [ScriptField]
        [ScriptName("length")]
        public extern int Count { get; }

        [ScriptField]
        public extern T this[int index] { get; set; }

        [ScriptField]
        extern object IList.this[int index] { get; set; }

        [ScriptName("push")]
        public extern void Add(T item);

        [ScriptName("push")]
        extern int IList.Add(object value);

        public extern bool Contains(T item);

        extern bool IList.Contains(object value);

        public extern void Clear();

        public extern int IndexOf(T item);

        extern int IList.IndexOf(object value);

        [DSharpScriptMemberName("remove")]
        public extern bool Remove(T item);

        [DSharpScriptMemberName("remove")]
        extern void IList.Remove(object value);

        public extern void RemoveAt(int index);

        public extern IEnumerator<T> GetEnumerator();

        extern IEnumerator IEnumerable.GetEnumerator();

        public extern void Insert(int index, T item);

        public extern void ForEach(Action<T> action);

        [ScriptSkip]
        public extern T[] ToArray();

        [ScriptSkip]
        public extern ReadOnlyCollection<T> AsReadOnly();

        public extern static explicit operator Array(List<T> list);

        public extern static explicit operator object[] (List<T> list);

        public extern static implicit operator T[] (List<T> list);

        public extern static explicit operator ArrayList(List<T> list);

        public extern static explicit operator List<T>(T[] array);
    }
}
