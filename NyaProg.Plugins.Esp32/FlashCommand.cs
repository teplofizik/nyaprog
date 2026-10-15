using System;
using System.Collections.Generic;
using System.IO;

using Esp32Flasher.Core;
using Esp32Flasher.Core.Chips;
using Esp32Flasher.Core.Imaging;
using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    /// <summary>
    /// Прошивка образов ESP32. Образы задаются парами <c>offset:файл</c> через ';':
    /// <c>images="0x1000:bootloader.bin;0x8000:partitions.bin;0x10000:firmware.bin"</c>.
    /// </summary>
    sealed class FlashCommand : CommandBase
    {
        public FlashCommand() : base("wflash", "Прошить образы ESP32")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("port", "str", true, null, "COM-порт"),
                new CommandParameter("images", null, false, null, "Пары offset:файл через ';'"),
                new CommandParameter("flash-size", null, false, "keep", "keep|detect|1MB|2MB|4MB|8MB|16MB"),
                new CommandParameter("flash-mode", null, false, "keep", "keep|qio|qout|dio|dout"),
                new CommandParameter("flash-freq", null, false, "keep", "keep|80m|40m|26m|20m"),
                new CommandParameter("baud", null, false, "921600"),
                new CommandParameter("verify", null, false, "1"),
                new CommandParameter("before", null, false, "default_reset", "default_reset|usb_reset|no_reset")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Port = Esp32Args.Port(Ctx);
            if (string.IsNullOrEmpty(Port))
                return CommandResult.Error("wflash: параметр 'port' обязателен");

            var Spec = Ctx.Args.Get("images");
            if (string.IsNullOrEmpty(Spec))
                return CommandResult.Error("wflash: параметр 'images' обязателен (offset:файл;...)");

            var Images = new List<FlashImage>();
            foreach (var Part in Spec.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var P = Part.Trim();
                if (P.Length == 0) continue;

                var Sep = P.IndexOf(':');
                if (Sep <= 0)
                    return CommandResult.Error($"wflash: ожидается offset:файл, получено '{P}'");

                var Offset = Esp32Args.ParseAddress(P.Substring(0, Sep));
                var FilePath = Esp32Args.ResolveFile(Ctx, P.Substring(Sep + 1).Trim());

                if (!File.Exists(FilePath))
                    return CommandResult.Error($"wflash: файл не найден: {FilePath}");

                Images.Add(FlashImage.FromFile(Offset, FilePath));
            }

            if (Images.Count == 0)
                return CommandResult.Error("wflash: не задано ни одного образа");

            FlashMode? Mode;
            FlashFrequency? Frequency;
            if (!Esp32Args.TryMode(Ctx.Args.Get("flash-mode"), out Mode))
                return CommandResult.Error("wflash: неверный flash-mode");
            if (!Esp32Args.TryFreq(Ctx.Args.Get("flash-freq"), out Frequency))
                return CommandResult.Error("wflash: неверный flash-freq");

            var Verify = Ctx.Args.Get("verify") != "0";
            var Baud = Esp32Args.Baud(Ctx, "baud", 921600);

            var Options = new EspConnectionOptions
            {
                PortName = Port,
                InitialBaud = 115200,
                FlashBaud = Baud,
                LoadStub = true,
                BeforeReset = Esp32Args.ParseBefore(Ctx.Args.Get("before"))
            };

            Esp32Connection Esp = null;
            try
            {
                Esp = Esp32Connection.OpenAsync(Options).GetAwaiter().GetResult();

                var SizeArg = Ctx.Args.Get("flash-size") ?? "keep";
                var Detect = string.Equals(SizeArg, "detect", StringComparison.OrdinalIgnoreCase);
                var FlashSizeBytes = 4 * 1024 * 1024u;
                FlashSize? Size = null;

                if (!string.Equals(SizeArg, "keep", StringComparison.OrdinalIgnoreCase) && !Detect)
                {
                    FlashSizeBytes = Esp32Args.ParseSize(SizeArg);
                    Size = Esp32Chip.SizeFromBytes(FlashSizeBytes);
                }
                else
                {
                    var Detected = Esp.DetectFlashSizeAsync().GetAwaiter().GetResult();
                    if (Detected is uint Bytes)
                    {
                        FlashSizeBytes = Bytes;
                        if (Detect) Size = Esp32Chip.SizeFromBytes(Bytes);
                    }
                }

                Esp.FlashAsync(Images, new FlashOptions
                {
                    Verify = Verify,
                    Reboot = true,
                    FlashSizeBytes = FlashSizeBytes,
                    Mode = Mode,
                    Frequency = Frequency,
                    Size = Size
                }).GetAwaiter().GetResult();

                var Out = new ArgumentList();
                Out.Set("chip", Esp.Chip.Name);
                Out.Set("mac", Esp.Chip.MacAddress);

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
