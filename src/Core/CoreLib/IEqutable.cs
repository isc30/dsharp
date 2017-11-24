using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    public interface IEquatable<T>
    {
        bool Equals(T other);
    }
}