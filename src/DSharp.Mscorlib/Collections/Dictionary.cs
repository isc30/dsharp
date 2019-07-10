using System.Runtime.CompilerServices;
// System.Collections.Dictionary doesn't exist in CLR. It does exist:
// System.Collections.IDictionary
// System.Collections.DictionaryBase
namespace System.Collections
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("Object")]
    // In CLR this is an interface and not a class, therefore no ctors are defined there
    public sealed partial class Dictionary : IEnumerable //In CLR this gui inherits from ICollection that inherits from IEnumerable
    {
        public Dictionary() { }

        public Dictionary(params object[] nameValuePairs) { }

        // Instead of string[] it should be ICollection

        // We miss the Values property
        public extern string[] Keys { get; }

        [ScriptField]
        public extern object this[string key] { get; set; }

        [DSharpScriptMemberName("clearKeys")]
        public extern void Clear();

        [DSharpScriptMemberName("keyExists")]
        public extern bool ContainsKey(string key);  // This method should look like this -> bool Contains(object key);

        public extern void Remove(string key);   // The key should be an object

        extern IEnumerator IEnumerable.GetEnumerator();  // This method should return IDictionaryEnumerator
    }
}
