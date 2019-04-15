namespace DSharp.Compiler.ScriptModel.Symbols
{
    internal sealed class GenericTypeSymbol : ITypeSymbol
    {
        public GenericTypeSymbol(int genericArgumentIndex, NamespaceSymbol parent)
            : base(SymbolType.GenericParameter, "<T>", parent)
        {
            GenericArgumentIndex = genericArgumentIndex;
        }

        public int GenericArgumentIndex { get; }
    }
}
