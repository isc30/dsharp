using System;

[assembly: ScriptAssembly("test")]

namespace Aadasdasd
{
    public static class AAAA
    {
        public static int What<T>(this object left, T right)
        {
            var x = typeof(T);
            return 6;
        }
    }
}

namespace LoweringTests
{
    using Aadasdasd;

    public static class Extensions
    {
        public static int DoSomething(this int left, int right)
        {
            return left + right;
        }

        public static int DoSomethingElse<T>(this object left, T right)
        {
            var x = typeof(T);
            return 6;
        }
    }

    public static class Main
    {
        public static int WOW = 666;

        public static void Main()
        {
            AAAA.What<int>(1, 2);
            AAAA.What<int>(AAAA.What<int>(1, 2), AAAA.What<int>(3, 4));

            int x = 3.DoSomething(5);

            var a = x.DoSomethingElse(WOW);
            var b = WOW.DoSomethingElse<double>(a).DoSomething(234).What(56);
        }
    }
}
