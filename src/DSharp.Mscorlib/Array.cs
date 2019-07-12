using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    // NOTE: Keep in sync with ArrayList and List
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Array")]
    public abstract partial class Array : IList
    {
        [ScriptField]
        public extern object this[int index]
        {
            get;
            set;
        }

        [ScriptField]
        [ScriptName("length")]
        public extern int Count { get; }

        [ScriptName("push")]
        public extern int Add(object value);

        public extern void Clear();

        public extern bool Contains(object value);

        public extern IEnumerator GetEnumerator();

        public extern int IndexOf(object value);

        [DSharpScriptMemberName("remove")]
        public extern void Remove(object value);

        public extern void RemoveAt(int index);

        public extern static explicit operator ArrayList(Array array);

        public extern static explicit operator List<object>(Array array);
    }
}
