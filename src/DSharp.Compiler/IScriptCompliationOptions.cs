using System.Collections.Generic;

namespace DSharp.Compiler
{
    public interface IScriptCompliationOptions
    {
        ICollection<string> Defines { get;  }

        IStreamSource DocCommentFile { get; }

        IStreamSourceResolver IncludeResolver { get; }

        bool Minimize { get; }

        ICollection<string> References { get; }

        ICollection<IStreamSource> Resources { get; }

        IStreamSource ScriptFile { get; }

        ICollection<IStreamSource> Sources { get; }

        string AssemblyName { get; }

        bool EnableDocComments { get; }
    }
}
