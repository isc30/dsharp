using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    public sealed class Random
    {
        public extern Random();

        public extern Random(int seed);

        [ScriptName("nextInt")]
        public extern int Next();

        [ScriptName("nextInt")]
        public extern int Next(int minValue, int maxValue);

        [ScriptName("nextIntMax")]
        public extern int Next(int maxValue);

        [ScriptName("nextDouble")]
        public extern double NextDouble();
    }
}