using System.Runtime.CompilerServices;

namespace System.Collections
{
    [ScriptImport]
    [ScriptName("IList")]
    public interface IList : ICollection
    {
        [ScriptField]
        object this[int index]
        {
            get;
            set;
        }

        [ScriptName("push")]
        int Add(object value);

        bool Contains(object value);

        void Clear();

        int IndexOf(object value);

        [DSharpScriptMemberName("remove")]
        void Remove(object value);

        void RemoveAt(int index);
    }
}
