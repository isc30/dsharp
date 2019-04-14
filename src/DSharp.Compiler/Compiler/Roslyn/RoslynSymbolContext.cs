using DSharp.Compiler.ScriptModel;
using Microsoft.CodeAnalysis;

namespace DSharp.Compiler.Roslyn
{
    internal class RoslynSymbolContext : ISymbolContext
    {
        public ISymbolResolver Resolver { get; set; }

        public IScriptModel ScriptModel { get; set; }

        public SemanticModel LocalSemanticModel { get; set; }

        public IAssemblyBuildContext BuildContext { get; set; }
    }
}
