namespace Programmer.Modules
{
    /// <summary>
    /// База для инструментов, реализованных кодом (встроенные утилиты, DLL-плагины).
    /// </summary>
    public abstract class CodedToolBase : ToolBase
    {
        private readonly ToolKind mKind;

        protected CodedToolBase(string Id, string Name, ToolKind Kind = ToolKind.Internal)
            : base(Id, Name)
        {
            mKind = Kind;
        }

        public override ToolKind Kind => mKind;
    }
}
