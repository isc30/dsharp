using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Collections
{
    //
    [Obsolete("Use List<T> instead")]
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Array")]
    public sealed partial class ArrayList : IList
    {
        public ArrayList() { }

        public ArrayList(int capacity) { }

        public ArrayList(params object[] items) { }

        [ScriptField]
        [ScriptName("length")]
        public extern int Count { get; }

        [ScriptField]
        public extern object this[int index] { get; set; }

        [ScriptName("push")]
        public extern int Add(object item);

        [ScriptName("push")]
        public extern void AddRange(params object[] items);

        public extern bool Contains(object value);

        public extern void Clear();

        public extern IEnumerator GetEnumerator();

        public extern int IndexOf(object item);

        public extern void RemoveAt(int index);

        [DSharpScriptMemberName("remove")]
        public extern void Remove(object value);

        public extern static implicit operator Array(ArrayList list);

        public extern static implicit operator object[](ArrayList list);

        public extern static implicit operator List<object>(ArrayList list);

        public extern static explicit operator ArrayList(object[] array);
    }
}
