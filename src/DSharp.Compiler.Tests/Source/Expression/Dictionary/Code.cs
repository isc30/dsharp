using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: ScriptAssembly("test")]

namespace ExpressionTests {

    public class App {

        public Dictionary GetData() {
            return null;
        }

        public Dictionary<string, int> GetData2() {
            return null;
        }

        public void Test(int arg) {
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>("xyz", 1, "abc", new App(), "delete", 2, "test.", 3, "\t", 4);
        }

        public void Test2(int arg) {
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>("aaa", 123, "xyz", true);
            string key = "blah";
            int c = dictionary1.Count;

            int c2 = GetData().Count;

            dictionary1.Remove("aaa");
            dictionary1.Remove("Proxy-Connection");
            dictionary1.Remove(key);
            dictionary1.Remove(new KeyValuePair<string, object>("xyz", true));

            dictionary1.Clear();

            // TODO: Fix
            dictionary1["asd"] = 3;
            object val = dictionary1["asd"];

            // TODO: Fix
            dictionary1.Add("hello", "bye");
            dictionary1.Add(new KeyValuePair<string, object>("hello2", "bye2"));

            // TODO: Fix
            dictionary1.ContainsKey("asd");
            dictionary1.Contains("asd");
            dictionary1.Contains(new KeyValuePair<string, object>("asd", 3));

            ICollection<string> keys = dictionary1.Keys;
            ICollection<object> values = dictionary1.Values;
        }

        public void Test3(int arg) {
            Dictionary<string, int> dictionary1 = new Dictionary<string, int>("aaa", 123, "xyz", true);
            string key = "blah";
            int c = dictionary1.Count;

            int c2 = GetData2().Count;

            bool b = dictionary1.ContainsKey("aaa");

            dictionary1.Remove("aaa");
            dictionary1.Remove("Proxy-Connection");
            dictionary1.Remove(key);

            dictionary1.Clear();
            
            string[] keys = dictionary1.Keys;
        }

        public void Test4(int arg) {
            Dictionary<int, string> d1 = new Dictionary<int, string>();
            d1[1] = d1[arg] = "aaa";
            d1.Remove(1);
            d1.Remove(arg);
        }
    }
}
