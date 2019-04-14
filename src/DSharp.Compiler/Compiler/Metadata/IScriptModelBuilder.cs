using System.Collections.Generic;
using DSharp.Compiler.ScriptModel;
using DSharp.Compiler.ScriptModel.Symbols;

namespace DSharp.Compiler.Metadata
{
    public interface IScriptModelBuilder<T>
    {
        ICollection<ITypeSymbol> BuildMetadata(T compilation, IScriptModel scriptModel, IScriptCompliationOptions options);
    }
}
