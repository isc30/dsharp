// ICollection.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{

    [ScriptImport]
    [ScriptName("ICollection")]
    public interface ICollection<T> : IEnumerable<T>
    {
        [ScriptField]
        [ScriptName("length")]
        int Count { get; }

        void Add(T item);

        void Clear();

        bool Contains(T item);

        bool Remove(T item);
    }
}
