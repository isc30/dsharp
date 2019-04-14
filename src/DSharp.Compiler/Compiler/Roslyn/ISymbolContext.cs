using DSharp.Compiler.ScriptModel;
using Microsoft.CodeAnalysis;

namespace DSharp.Compiler.Roslyn
{
    public interface ISymbolContext
    {
        ISymbolResolver Resolver { get; }

        IScriptModel ScriptModel { get; }

        SemanticModel LocalSemanticModel { get; }

        IAssemblyBuildContext BuildContext { get; }
    }
}
