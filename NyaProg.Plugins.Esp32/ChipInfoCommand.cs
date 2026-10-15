using System;

using Esp32Flasher.Core;
using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    /// <summary>
    /// Определение чипа ESP32: имя, ревизия, MAC (в выходные переменные).
    /// </summary>
    sealed class ChipInfoCommand : CommandBase
    {
        public ChipInfoCommand() : base("chipinfo", "Инфо о чипе ESP32")
        {
            Descriptor.Parameters = new System.Collections.Generic.List<CommandParameter>
            {
                new CommandParameter("port", "str", true, null, "COM-порт"),
                new CommandParameter("baud", null, false, "115200"),
                new CommandParameter("before", null, false, "default_reset", "default_reset|usb_reset|no_reset")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Port = Esp32Args.Port(Ctx);
            if (string.IsNullOrEmpty(Port))
                return CommandResult.Error("chipinfo: параметр 'port' обязателен");

            var Options = new EspConnectionOptions
            {
                PortName = Port,
                InitialBaud = Esp32Args.Baud(Ctx, "baud", 115200),
                FlashBaud = null,
                LoadStub = true,
                BeforeReset = Esp32Args.ParseBefore(Ctx.Args.Get("before"))
            };

            Esp32Connection Esp = null;
            try
            {
                Esp = Esp32Connection.OpenAsync(Options).GetAwaiter().GetResult();

                var C = Esp.Chip;
                var Out = new ArgumentList();
                Out.Set("chip", C.Name);
                Out.Set("revision", $"v{C.RevisionMajor}.{C.RevisionMinor}");
                Out.Set("mac", C.MacAddress);

                return new CommandResult(CommandStatus.Ok, "", Out);
            }
            catch (Exception E)
            {
                return CommandResult.Error(E.Message);
            }
            finally
            {
                if (Esp != null) Esp.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
    }
}
