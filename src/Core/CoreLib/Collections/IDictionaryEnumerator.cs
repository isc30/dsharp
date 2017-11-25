using System.Runtime.CompilerServices;

namespace System.Collections
{
    [ScriptImport]
    public interface IDictionaryEnumerator : IEnumerator
    {
        object Key { get; }

        object Value { get; }

        DictionaryEntry Entry
        {
            get;
        }
    }
}