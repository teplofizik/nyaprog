using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.HexConv
{
    /// <summary>
    /// Плагин конвертации форматов: bin ↔ Intel HEX.
    /// </summary>
    public sealed class HexConvModule : ModuleBase
    {
        public override string Id => "hexconv";

        public override string Name => "HEX converter";

        public override IEnumerable<ITool> GetTools() => new ITool[] { new HexConvTool() };
    }

    public sealed class HexConvTool : CodedToolBase
    {
        public HexConvTool() : base("hexconv.tool", "HEX", ToolKind.Internal)
        {
            AddCommand(new BinToHexCommand());
            AddCommand(new HexToBinCommand());
        }
    }

    abstract class ConvCommand : CommandBase
    {
        protected ConvCommand(string Id, string Title) : base(Id, Title) { }

        protected static string Resolve(CommandContext Ctx, string File)
        {
            if (Path.IsPathRooted(File)) return File;
            return Path.Combine(Ctx.WorkingDir ?? "", File);
        }

        protected static int ParseOffset(string Text)
        {
            Text = (Text ?? "0").Trim();
            if (Text.Length == 0) return 0;

            try
            {
                if (Text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    return int.Parse(Text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return int.Parse(Text, CultureInfo.InvariantCulture);
            }
            catch { return 0; }
        }

        protected static byte ParseByte(string Text)
        {
            Text = (Text ?? "0xFF").Trim();
            try
            {
                if (Text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    return byte.Parse(Text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return byte.Parse(Text, CultureInfo.InvariantCulture);
            }
            catch { return 0xFF; }
        }
    }

    /// <summary>bintohex: bin → Intel HEX (со стартовым адресом).</summary>
    sealed class BinToHexCommand : ConvCommand
    {
        public BinToHexCommand() : base("bintohex", "BIN → Intel HEX")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("in", "str", true, null, "Входной bin"),
                new CommandParameter("out", "str", true, null, "Выходной hex"),
                new CommandParameter("offset", null, false, "0", "Стартовый адрес (0x...)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var In = Ctx.Args.Get("in");
            var Out = Ctx.Args.Get("out");
            if (string.IsNullOrEmpty(In)) return CommandResult.Error("bintohex: параметр 'in' обязателен");
            if (string.IsNullOrEmpty(Out)) return CommandResult.Error("bintohex: параметр 'out' обязателен");

            byte[] Data;
            try { Data = Ctx.Host.Files.ReadAllBytes(Resolve(Ctx, In)); }
            catch (Exception E) { return CommandResult.Error($"bintohex: не прочитан '{In}': {E.Message}"); }

            var Offset = ParseOffset(Ctx.Args.Get("offset"));

            try { Ctx.Host.Files.WriteAllText(Resolve(Ctx, Out), IntelHexCodec.Build(Data, Offset)); }
            catch (Exception E) { return CommandResult.Error($"bintohex: не записан '{Out}': {E.Message}"); }

            var Outputs = new ArgumentList();
            Outputs.Set("file", Resolve(Ctx, Out));
            Outputs.Set("size", Data.Length.ToString());
            Outputs.Set("offset", "0x" + Offset.ToString("X8"));

            return new CommandResult(CommandStatus.Ok, Resolve(Ctx, Out), Outputs);
        }
    }

    /// <summary>hextobin: Intel HEX → bin.</summary>
    sealed class HexToBinCommand : ConvCommand
    {
        public HexToBinCommand() : base("hextobin", "Intel HEX → BIN")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("in", "str", true, null, "Входной hex"),
                new CommandParameter("out", "str", true, null, "Выходной bin"),
                new CommandParameter("offset", null, false, "0", "Базовый адрес результата (0 — авто)"),
                new CommandParameter("fill", null, false, "0xFF", "Заполнение пропусков")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var In = Ctx.Args.Get("in");
            var Out = Ctx.Args.Get("out");
            if (string.IsNullOrEmpty(In)) return CommandResult.Error("hextobin: параметр 'in' обязателен");
            if (string.IsNullOrEmpty(Out)) return CommandResult.Error("hextobin: параметр 'out' обязателен");

            string Text;
            try { Text = Ctx.Host.Files.ReadAllText(Resolve(Ctx, In)); }
            catch (Exception E) { return CommandResult.Error($"hextobin: не прочитан '{In}': {E.Message}"); }

            int Base;
            var Data = IntelHexCodec.Parse(Text, ParseOffset(Ctx.Args.Get("offset")), ParseByte(Ctx.Args.Get("fill")), out Base);

            try { Ctx.Host.Files.WriteAllBytes(Resolve(Ctx, Out), Data); }
            catch (Exception E) { return CommandResult.Error($"hextobin: не записан '{Out}': {E.Message}"); }

            var Outputs = new ArgumentList();
            Outputs.Set("file", Resolve(Ctx, Out));
            Outputs.Set("size", Data.Length.ToString());
            Outputs.Set("base", "0x" + Base.ToString("X8"));

            return new CommandResult(CommandStatus.Ok, Resolve(Ctx, Out), Outputs);
        }
    }
}
