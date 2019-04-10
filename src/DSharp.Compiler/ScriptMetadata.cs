namespace DSharp.Compiler
{
    public class ScriptMetadata
    {
        public string ScriptName { get; set; }

        public string Copyright { get; set; }

        public string Description { get; set; }

        public string Template { get; set; } = DSharpStringResources.DEFAULT_SCRIPT_TEMPLATE;

        public string Version { get; set; }

        public bool EnableDocComments { get; set; }
    }
}
