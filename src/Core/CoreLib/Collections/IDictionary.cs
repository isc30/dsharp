using System.Runtime.CompilerServices;

namespace System.Collections
{
    [ScriptImport]
    public interface IDictionary : ICollection
    {
        object this[object key] { get; set; }

        ICollection Keys { get; }

        ICollection Values { get; }

        bool Contains(object key);

        void Add(object key, object value);

        void Clear();

        new IDictionaryEnumerator GetEnumerator();

        void Remove(object key);
    }
}