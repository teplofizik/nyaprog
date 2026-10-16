using System.Collections.Generic;

using Programmer.Modules;

namespace NyaProg.Plugins.SettingsGen
{
    /// <summary>
    /// Плагин генерации блока настроек (EEPROM) из XML-описания:
    /// типы полей (u8..u64 LE/BE, MAC, IP, строки, hex, паддинг) + контрольные
    /// суммы (sum/CRC) и хэши (MD5/SHA-256). Результат — bin или Intel HEX по offset.
    /// </summary>
    public sealed class SettingsGenModule : ModuleBase
    {
        public override string Id => "settingsgen";

        public override string Name => "Settings generator";

        public override IEnumerable<ITool> GetTools() => new ITool[] { new SettingsGenTool() };
    }

    public sealed class SettingsGenTool : CodedToolBase
    {
        public SettingsGenTool() : base("settingsgen.tool", "Settings", ToolKind.Internal)
        {
            AddCommand(new SettingsCommand());
        }
    }
}
