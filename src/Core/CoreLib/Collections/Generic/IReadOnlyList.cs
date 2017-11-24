using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    [ScriptImport]
    public interface IReadOnlyList<out T> : IReadonlyCollection<T>
    {
        T this[int index] { get; }
    }
}