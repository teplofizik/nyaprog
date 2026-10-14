using System.Collections.Generic;

using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.Sample
{
    /// <summary>
    /// Пример стороннего DLL-плагина: команда echo.
    /// Собирается отдельно и кладётся в каталог plugins.
    /// </summary>
    public sealed class SampleModule : ModuleBase
    {
        public override string Id => "sample";

        public override string Name => "Sample plugin";

        public override IEnumerable<ITool> GetTools() => new ITool[]
        {
            new SampleTool()
        };
    }

    public sealed class SampleTool : CodedToolBase
    {
        public SampleTool() : base("sample.tool", "Sample", ToolKind.Utility)
        {
            AddCommand(new EchoCommand());
            AddCommand(new ScanCommand());
        }
    }

    sealed class EchoCommand : CommandBase
    {
        public EchoCommand() : base("echo", "Эхо") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Outputs = new ArgumentList();
            Outputs.Set("echo", Ctx.Args.Get("value") ?? "");

            return new CommandResult(CommandStatus.Ok, "", Outputs);
        }
    }

    /// <summary>
    /// Пример команды-поставщика динамического списка: «найденные устройства».
    /// </summary>
    sealed class ScanCommand : CommandBase
    {
        public ScanCommand() : base("scan.devices", "Сканировать устройства") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Items = new List<CommandItem>
            {
                new CommandItem("Устройство A", Args("device", "AA:BB:CC")),
                new CommandItem("Устройство B", Args("device", "DD:EE:FF")),
                new CommandItem("Устройство C", Args("device", "11:22:33")),
            };

            return CommandResult.List(Items);
        }

        private static ArgumentList Args(string K, string V)
        {
            var A = new ArgumentList();
            A.Set(K, V);
            return A;
        }
    }
}
