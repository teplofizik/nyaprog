using System;
using System.Collections.Generic;

using Esp32Flasher.Core;
using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    /// <summary>
    /// Стирание всего flash-чипа ESP32.
    /// </summary>
    sealed class EraseCommand : CommandBase
    {
        public EraseCommand() : base("erase", "Стереть весь чип ESP32")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("port", "str", true, null, "COM-порт"),
                new CommandParameter("baud", null, false, "921600"),
                new CommandParameter("before", null, false, "default_reset", "default_reset|usb_reset|no_reset")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Port = Esp32Args.Port(Ctx);
            if (string.IsNullOrEmpty(Port))
                return CommandResult.Error("erase: параметр 'port' обязателен");

            var Options = new EspConnectionOptions
            {
                PortName = Port,
                InitialBaud = 115200,
                FlashBaud = Esp32Args.Baud(Ctx, "baud", 921600),
                LoadStub = true,
                BeforeReset = Esp32Args.ParseBefore(Ctx.Args.Get("before"))
            };

            Esp32Connection Esp = null;
            try
            {
                Esp = Esp32Connection.OpenAsync(Options).GetAwaiter().GetResult();
                Esp.EraseFlashAsync().GetAwaiter().GetResult();

                return CommandResult.Ok();
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
