using System;
using System.IO;

using Esp32Flasher.Core.Imaging;
using Esp32Flasher.Core.Reset;
using Programmer.Modules;

namespace NyaProg.Plugins.Esp32
{
    static class Esp32Args
    {
        public static string Port(CommandContext Ctx) => Ctx.Args.Get("port");

        public static int Baud(CommandContext Ctx, string Name, int Default)
        {
            var V = Ctx.Args.Get(Name);
            int R;
            return ((V != null) && int.TryParse(V, out R)) ? R : Default;
        }

        public static uint ParseAddress(string Text)
        {
            Text = (Text ?? "").Trim();

            if (Text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return Convert.ToUInt32(Text.Substring(2), 16);

            return uint.Parse(Text);
        }

        public static uint ParseSize(string Text)
        {
            var T = (Text ?? "").Trim().ToUpperInvariant();

            if (T.EndsWith("MB", StringComparison.Ordinal))
                return (uint)uint.Parse(T.Substring(0, T.Length - 2)) * 1024 * 1024;

            if (T.EndsWith("KB", StringComparison.Ordinal))
                return (uint)uint.Parse(T.Substring(0, T.Length - 2)) * 1024;

            return ParseAddress(T);
        }

        public static string ResolveFile(CommandContext Ctx, string File)
        {
            if (Path.IsPathRooted(File)) return File;
            return Path.Combine(Ctx.WorkingDir ?? "", File);
        }

        public static ResetMode ParseBefore(string Text)
        {
            switch ((Text ?? "").Trim().ToLowerInvariant())
            {
                case "usb":
                case "usb_reset":
                case "usb-jtag": return ResetMode.UsbJtag;
                case "no":
                case "no_reset": return ResetMode.NoReset;
                default: return ResetMode.Default;
            }
        }

        public static bool TryMode(string Text, out FlashMode? Mode)
        {
            switch ((Text ?? "keep").Trim().ToLowerInvariant())
            {
                case "keep": Mode = null; return true;
                case "qio": Mode = FlashMode.Qio; return true;
                case "qout": Mode = FlashMode.Qout; return true;
                case "dio": Mode = FlashMode.Dio; return true;
                case "dout": Mode = FlashMode.Dout; return true;
                default: Mode = null; return false;
            }
        }

        public static bool TryFreq(string Text, out FlashFrequency? Frequency)
        {
            switch ((Text ?? "keep").Trim().ToLowerInvariant())
            {
                case "keep": Frequency = null; return true;
                case "80m": Frequency = FlashFrequency.MHz80; return true;
                case "40m": Frequency = FlashFrequency.MHz40; return true;
                case "26m": Frequency = FlashFrequency.MHz26; return true;
                case "20m": Frequency = FlashFrequency.MHz20; return true;
                default: Frequency = null; return false;
            }
        }
    }
}
