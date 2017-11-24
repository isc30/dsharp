using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    public interface IEqualityComparer<in T>
    {
        bool Equals(T x, T y);
    }
}