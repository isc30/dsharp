namespace System.Runtime.CompilerServices
{
    [ScriptImport]
    [ScriptName("ss")]
    public static class RuntimeHelper
    {
        [ScriptName("getHashCode")]
        public extern static int GetHashCode(object obj);
    }
}