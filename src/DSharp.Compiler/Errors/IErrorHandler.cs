namespace DSharp.Compiler.Errors
{
    public interface IErrorHandler
    {
        bool HasErrors { get; }

        void ReportError(CompilerError error);
    }
}
