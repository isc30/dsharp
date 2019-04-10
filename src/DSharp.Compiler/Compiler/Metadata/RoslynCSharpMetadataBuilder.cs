using System.Collections.Generic;
using DSharp.Compiler.ScriptModel;
using DSharp.Compiler.ScriptModel.Symbols;
using Microsoft.CodeAnalysis.CSharp;

namespace DSharp.Compiler.Metadata
{
    public class RoslynCSharpMetadataBuilder : IScriptModelBuilder<CSharpCompilation>
    {
        public ICollection<TypeSymbol> BuildMetadata(
            CSharpCompilation compilation,
            IScriptModel scriptModel,
            IScriptCompliationOptions options)
        {
            return null;
        }
    }
}
