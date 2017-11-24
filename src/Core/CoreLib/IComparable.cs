using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    public interface IComparable
    {
        int CompareTo(object obj);
    }

    [ScriptImport]
    public interface IComparable<in T>
    {
        int CompareTo(T other);
    }
}