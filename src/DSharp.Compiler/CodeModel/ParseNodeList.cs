// ParseNodeList.cs
// Script#/Core/Compiler
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Collections;

namespace DSharp.Compiler.CodeModel
{
    public sealed class ParseNodeList : IEnumerable
    {
        private ArrayList list;

        public ParseNodeList()
        {
        }

        public ParseNodeList(ParseNode node)
        {
            Append(node);
        }

        public int Count
        {
            get
            {
                if (list == null)
                {
                    return 0;
                }

                return list.Count;
            }
        }

        public ParseNode this[int index] => (ParseNode) list[index];

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Append(ParseNode node)
        {
            if (node != null)
            {
                EnsureListCreated();
                list.Add(node);
            }
        }

        public void Append(ParseNodeList nodes)
        {
            EnsureListCreated();
            list.AddRange(nodes.list);
        }

        private void EnsureListCreated()
        {
            if (list == null)
            {
                list = new ArrayList();
            }
        }

        public ParseNodeEnumerator GetEnumerator()
        {
            return new ParseNodeEnumerator(this);
        }

        internal void SetParent(ParseNode parent)
        {
            if (list != null)
            {
                list.TrimToSize();
                foreach (ParseNode child in this) child.SetParent(parent);
            }
        }

        public sealed class ParseNodeEnumerator : IEnumerator
        {
            private readonly ArrayList list;
            private int current;

            public ParseNodeEnumerator(ParseNodeList nodes)
            {
                list = nodes.list;
            }

            public ParseNode Current => (ParseNode) list[current - 1];

            public bool MoveNext()
            {
                if (list != null && current < list.Count)
                {
                    current += 1;

                    return true;
                }

                return false;
            }

            #region Implementation of IEnumerator

            object IEnumerator.Current => Current;

            bool IEnumerator.MoveNext()
            {
                return MoveNext();
            }

            void IEnumerator.Reset()
            {
                current = 0;
            }

            #endregion
        }
    }
}
