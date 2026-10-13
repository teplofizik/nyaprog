using Programmer.Modules;

namespace Programmer.Modules.Internal
{
    /// <summary>
    /// Набор встроенных (кодовых) инструментов ядра.
    /// </summary>
    public static class InternalModule
    {
        public static ITool[] CreateAll() => new ITool[]
        {
            new FileOpsTool(),
            new MessageTool(),
            new ExternalTool()
        };
    }
}
