using System.Collections.Generic;

using Programmer.Modules;

namespace Programmer.Modules.Net
{
    /// <summary>
    /// Модуль сетевых инструментов: download (файл) и lookup (значения в переменные).
    /// </summary>
    public sealed class NetModule : ModuleBase
    {
        public override string Id => "core.net";

        public override string Name => "Net";

        public override IEnumerable<ITool> GetTools() => new ITool[]
        {
            new NetTool()
        };
    }

    public sealed class NetTool : CodedToolBase
    {
        public NetTool() : base("core.net.tool", "Net")
        {
            AddCommand(new DownloadCommand());
            AddCommand(new LookupCommand());
        }
    }
}
