using Programmer.Modules;

namespace Programmer.Modules.Net
{
    /// <summary>
    /// Сетевые инструменты: download (файл) и lookup (значения в переменные).
    /// </summary>
    public static class NetModule
    {
        public static ITool[] CreateAll() => new ITool[]
        {
            new NetTool()
        };
    }

    public sealed class NetTool : CodedToolBase
    {
        public NetTool() : base("core.net", "Net")
        {
            AddCommand(new DownloadCommand());
            AddCommand(new LookupCommand());
        }
    }
}
