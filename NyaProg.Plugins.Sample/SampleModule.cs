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
}
