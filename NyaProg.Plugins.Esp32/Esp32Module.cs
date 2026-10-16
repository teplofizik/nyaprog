using System.Collections.Generic;

using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    /// <summary>
    /// Плагин-программатор ESP32 на базе Esp32Flasher.Core.
    /// </summary>
    public sealed class Esp32Module : ModuleBase
    {
        public override string Id => "esp32";

        public override string Name => "ESP32 (native)";

        public override IEnumerable<ITool> GetTools() => new ITool[] { new Esp32Tool() };
    }

    public sealed class Esp32Tool : CodedToolBase
    {
        public Esp32Tool() : base("esp32.tool", "ESP32", ToolKind.Programmer)
        {
            AddArch("esp32");
            AddArch("esp32s3");
            AddArch("esp32c3");
            AddArch("esp32c6");

            AddCommand(new ScanPortsCommand());
            AddCommand(new ChipInfoCommand());
            AddCommand(new CheckCommand());
            AddCommand(new FlashCommand());
            AddCommand(new EraseCommand());
        }
    }
}
