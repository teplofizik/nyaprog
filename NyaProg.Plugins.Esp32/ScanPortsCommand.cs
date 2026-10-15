using System.Collections.Generic;

using Esp32Flasher.Core.Serial;
using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    /// <summary>
    /// Команда-поставщик: список COM-портов (для динамической опции выбора порта).
    /// Если порт в системе один — он окажется единственным пунктом списка (авто-выбор).
    /// </summary>
    sealed class ScanPortsCommand : CommandBase
    {
        public ScanPortsCommand() : base("scan.ports", "Список COM-портов") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Items = new List<CommandItem>();

            foreach (var P in SerialPorts.List())
            {
                var A = new ArgumentList();
                A.Set("port", P.PortName);

                var Label = string.IsNullOrEmpty(P.Description) ? P.PortName : $"{P.PortName} — {P.Description}";
                Items.Add(new CommandItem(Label, A));
            }

            return CommandResult.List(Items);
        }
    }
}
