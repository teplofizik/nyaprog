using System;
using System.Collections.Generic;

using Esp32Flasher.Core;
using Esp32Flasher.Core.Chips;
using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    /// <summary>
    /// Проверка контроллера перед прошивкой: сверяет имя чипа и размер flash
    /// с ожидаемыми. При несовпадении — ошибка (шаг и прогон прерываются).
    /// </summary>
    sealed class CheckCommand : CommandBase
    {
        public CheckCommand() : base("check", "Проверка контроллера")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("port", "str", true, null, "COM-порт"),
                new CommandParameter("chip", null, false, null, "Ожидаемое имя чипа (подстрока), напр. ESP32"),
                new CommandParameter("flash-size", null, false, null, "Ожидаемый размер flash, напр. 4MB"),
                new CommandParameter("baud", null, false, "115200"),
                new CommandParameter("before", null, false, "default_reset", "default_reset|usb_reset|no_reset")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Port = Esp32Args.Port(Ctx);
            if (string.IsNullOrEmpty(Port))
                return CommandResult.Error("check: параметр 'port' обязателен");

            var ExpectChip = Ctx.Args.Get("chip");
            var ExpectSize = Ctx.Args.Get("flash-size");

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

                var Chip = Esp.Chip;
                var FlashId = Esp.ReadFlashIdAsync().GetAwaiter().GetResult();
                var SizeBytes = Esp32Chip.DecodeFlashSizeBytes(FlashId);

                if (!string.IsNullOrEmpty(ExpectChip) &&
                    (Chip.Name.IndexOf(ExpectChip, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    return CommandResult.Error(
                        $"check: чип '{Chip.Name}' не соответствует ожидаемому '{ExpectChip}'");
                }

                if (!string.IsNullOrEmpty(ExpectSize))
                {
                    if (SizeBytes == null)
                        return CommandResult.Error("check: не удалось определить размер flash");

                    var Expected = Esp32Args.ParseSize(ExpectSize);
                    if (SizeBytes.Value != Expected)
                    {
                        return CommandResult.Error(
                            $"check: размер flash {SizeBytes.Value / 1024 / 1024}MB не соответствует ожидаемому {Expected / 1024 / 1024}MB");
                    }
                }

                var Out = new ArgumentList();
                Out.Set("chip", Chip.Name);
                Out.Set("revision", $"v{Chip.RevisionMajor}.{Chip.RevisionMinor}");
                Out.Set("mac", Chip.MacAddress);
                if (SizeBytes != null) Out.Set("flash", $"{SizeBytes.Value / 1024 / 1024}MB");

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
