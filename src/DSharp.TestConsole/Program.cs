using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DSharp.Compiler;

namespace DSharp.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            const string TestScript = @"
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using DSharp;

[assembly: ScriptAssembly(Base.Consts.AssemblyName)]
[assembly: ScriptTemplate(""{script}"")]

[assembly: AssemblyCopyright(""Fred"")]
[assembly: AssemblyDescription(""AWESOME LIB"")]
[assembly: AssemblyFileVersion(""0.0.0.1"")]

public class GlobalBoi
{
    public float semi = 1.0f;
}

namespace Base
{
    public static class Consts
    {
        public const string AssemblyName = ""TestScript"";
    }

    public class Lazy<T>
    {
        private readonly Func<T> resolver;
        private T resolvedValue;
        private bool hasResolved = false;

        public Lazy(Func<T> resolver)
        {
            this.resolver = resolver;
        }

        public T Value
        {
            get 
            { 
                if(hasResolved)
                    return resolvedValue;

                resolvedValue = resolver.Invoke();
                hasResolved = true;
                return resolvedValue;
            }
        }
    }
}

namespace Deeply.Nested.SoThat.We.Have.A.Long.One
{
    public partial class AnotherOne
    {
        public void DoSomething() { }
    }

    public partial class AnotherOne
    {
        public string Value = 1;
    }
}

namespace Awesome
{
    public class SuperClass
    {
        public int Value = 1;
    }
}

namespace TestApp
{
    using Base;
    using Temp = Awesome.SuperClass;

    public class Program
    {
        public void Main(string[] args)
        {
            string result = ""someValue"";
            string constResult = Consts.AssemblyName;
            int classValue = new Temp().Value;
            TestObject to = new TestObject();
            result = to.Value;
        }
    }

    public class TestObject
    {
        public string Value { get { return ""Nothing""; }}
    }
}
";

            var outputStream = new InMemoryStream();
            var options = new CompilerOptions
            {
                AssemblyName = "DSharp.Test",
                References = new List<string>
                {
                    $@"C:\Projects\dsharp\src\DSharp.Mscorlib\bin\Debug\netstandard2.0\DSharp.Mscorlib.dll",
                },
                Defines = new[] { "SCRIPTSHARP" },
                Sources = new[]
                {
                    new ScriptLocalStreamSource(TestScript)
                },
                ScriptFile = outputStream
            };
            var compiler = new ScriptCompiler();
            compiler.Compile(options);

            Console.WriteLine(outputStream.GeneratedOutput);
        }

        internal class ScriptLocalStreamSource : IStreamSource
        {
            private readonly string script;

            public ScriptLocalStreamSource(string script)
            {
                this.script = script;
            }

            public string FullName { get; } = Guid.NewGuid().ToString();

            public string Name { get; }

            public Stream GetStream()
            {
                var memoryStream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8, script.Length, true))
                {
                    writer.AutoFlush = true;
                    writer.Write(script);
                }
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        public sealed class InMemoryStream : IStreamSource
        {
            private readonly string name;

            public string GeneratedOutput { get; private set; }

            string IStreamSource.FullName
            {
                get { return name; }
            }

            string IStreamSource.Name
            {
                get { return name; }
            }

            public InMemoryStream() => name = Guid.NewGuid().ToString();

            public Stream GetStream()
            {
                return new NotifiableMemoryStream
                {
                    OnBeforeDispose = HandleOnDisposed
                };
            }

            private void HandleOnDisposed(byte[] buffer, long length)
            {
                GeneratedOutput = Encoding.UTF8.GetString(buffer, 0, (int)length);
            }

            private class NotifiableMemoryStream : MemoryStream
            {
                private bool hasReadBuffer = false;
                public Action<byte[], long> OnBeforeDispose { get; set; }

                protected override void Dispose(bool disposing)
                {
                    if(!hasReadBuffer)
                    {
                        var buffer = GetBuffer();
                        OnBeforeDispose?.Invoke(buffer, Length);
                        hasReadBuffer = true;
                    }
                    
                    base.Dispose(disposing);
                }
            }
        }
    }
}
