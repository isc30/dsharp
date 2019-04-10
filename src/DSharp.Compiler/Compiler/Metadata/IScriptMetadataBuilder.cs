using DSharp.Compiler.ScriptModel;

namespace DSharp.Compiler.Metadata
{
    public interface IScriptMetadataBuilder<T>
    {
        ScriptMetadata Build(T compilation, IScriptModel scriptModel, IScriptCompliationOptions options);
    }
}
