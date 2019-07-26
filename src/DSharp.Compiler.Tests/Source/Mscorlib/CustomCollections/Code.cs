using System.Collections;
using System.Collections.Generic;

[assembly: ScriptAssembly("test")]

namespace MscorlibTests
{
    public class MyCollection : ICollection<int>
    {
        public int Count { get { return 2; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(int item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(int item)
        {
            return false;
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
        }

        public IEnumerator<int> GetEnumerator()
        {
            return null;
        }

        public bool Remove(int item)
        {
            return false;
        }

        [ScriptIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }

    public static class X
    {
        public static void TestList()
        {
            List<int> c = new List<int>();
            c.Add(12);
            c.AddRange(c); // remove method
            c.Clear();
            c.Contains(12);
            c.Remove(12);
            c.RemoveAt(1);
            IEnumerator<int> e = c.GetEnumerator();
            //bool eq = c.Equals(c);
            int i = c.IndexOf(12);
            c.ForEach(delegate (int n) { ++n; });
            int cc = c.Count;
            //c.Insert(1, 22);
        }

        public static void TestIList_T()
        {
            IList<int> c = new List<int>();
            int n = c[0];
            c[0] = c[1];
            c.Add(12);
            c.Clear();
            c.Contains(12);
            c.Remove(12);
            c.RemoveAt(1);
            IEnumerator<int> e = c.GetEnumerator();
            //bool eq = c.Equals(c);
            int i = c.IndexOf(12);
            int cc = c.Count;
            //c.Insert(1, 22);
        }

        public static void TestIList()
        {
            IList c = new List<int>();
            int n = c[0];
            c[0] = c[1];
            c.Add(12);
            c.Clear();
            c.Contains(12);
            c.Remove(12);
            c.RemoveAt(1);
            IEnumerator e = c.GetEnumerator();
            //bool eq = c.Equals(c);
            int i = c.IndexOf(12);
            int cc = c.Count;
            //c.Insert(1, 22);
        }

        public static void TestIReadOnlyList()
        {
            IList c = new List<int>();
            object n = c[0];
            c[0] = c[1];
            c.Add(12);
            c.Clear();
            c.Contains(12);
            c.Remove(12);
            c.RemoveAt(1);
            IEnumerator e = c.GetEnumerator();
            //bool eq = c.Equals(c);
            int i = c.IndexOf(12);
            int cc = c.Count;
            //c.Insert(1, 22);
        }
    }
}
