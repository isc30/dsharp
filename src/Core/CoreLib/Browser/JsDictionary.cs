using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Browser
{
    [ScriptImport]
    [ScriptName(object.NAME_DEFINITION)]
    public sealed class JsDictionary : object
    {
        public extern object this[string key] { get; set; }

        [ScriptAlias("ss.getValues")]
        public extern ICollection<object> Values { get; }

        [ScriptAlias("ss.keyCount")]
        public extern int Count { get; }

        [ScriptAlias("ss.keys")]
        public ICollection<string> Keys { get; }

        [ScriptAlias("ss.clearKeys")]
        public extern void Clear();

        [ScriptAlias("ss.keyExists")]
        public extern bool ContainsKey(string key);

        [ScriptAlias("ss.removeKey")]
        public extern bool Remove(string key);
    }
}