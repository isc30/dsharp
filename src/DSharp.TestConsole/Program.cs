using System.Collections.Generic;
using System.IO;
using System.Linq;
using DSharp.Compiler;

namespace DSharp.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            const string directory = @"C:\dev\Ace\ace.framework.master";
            string outputDir = @$"{directory}\Ace.Framework\Ace.Foundations\bin\Debug\DSharp\";

            var sourceFiles = Directory.EnumerateFiles($@"{directory}\Ace.Framework\Ace.Foundations", "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains(".g.cs"));

            var options = new CompilerOptions
            {
                AssemblyName = "DSharp.Test",
                References = new List<string>
                {
                    $@"C:\dev\dsharp\dsharp\src\DSharp.Mscorlib\bin\Debug\net461\DSharp.Mscorlib.dll",
                    $"{outputDir}ScriptSharp.Attribute.DSharp.dll",
                    $"{outputDir}Ace.Library.Build.CompilerServices.DSharp.dll",
                    $"{outputDir}Ace_Loader.dll"
                },
                Defines = new[] { "SCRIPTSHARP" },
                Sources = sourceFiles.Select(sf => new FileInputStreamSource(sf)).ToList<IStreamSource>()
            };
            var compiler = new ScriptCompiler();
            compiler.Compile(options);
        }

        internal class FileInputStreamSource : IStreamSource
        {
            private readonly string path;

            public FileInputStreamSource(string path)
                : this(path, path)
            {
            }

            public FileInputStreamSource(string path, string name)
            {
                this.path = path;
                Name = name;
            }

            public string FullName => Path.GetFullPath(path);

            public string Name { get; }

            public void CloseStream(Stream stream)
            {
                stream.Close();
            }

            public Stream GetStream()
            {
                try
                {
                    return new FileStream(FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
