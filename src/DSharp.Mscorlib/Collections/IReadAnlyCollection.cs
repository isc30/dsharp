using System.Runtime.CompilerServices;

namespace System.Collections
{
    [ScriptImport]
    [ScriptName("ICollection")]
    public interface IReadOnlyCollection : IEnumerable
    {
        [ScriptField]
        [ScriptName("length")]
        int Count { get; }

        [ScriptField]
        object this[int index] { get; }
    }
}
