using System;
using System.Collections;

namespace ExpressionTests
{
    public class DisposeMe : Program, IDisposable { public void Dispose() { } }

    public class Program
    {
        public void Main()
        {
            AAAAA(new DisposeMe(), new DisposeMe());
        }

        public void NoParameters<T>()
        {
        }

        public void NoParametersConstrained<T>(T x)
            where T : IDisposable
        {
            // this doesn't work
            Type t = typeof(T);

            // this works now
            x.Dispose();
        }

        public object InParameter<T>(T inGenericParameter)
        {
            return inGenericParameter;
        }

        public void AAAAA<T, Y>(T a1, Y a2)
            where T : IDisposable
            where Y : Program, IDisposable
        {
            a1.Dispose();
            a2.Main();
            a2.Dispose();
        }
    }
}
