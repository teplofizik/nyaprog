using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Extension.Argument;
using NyaProg.Plugins.SettingsGen.Settings;
using Programmer.Modules;

namespace NyaProg.Plugins.SettingsGen
{
    /// <summary>
    /// settings: сгенерировать блок настроек из XML-описания и сохранить в bin/hex.
    /// Параметры: file (описание), out (результат), format (hex|bin), offset, set.
    /// </summary>
    sealed class SettingsCommand : CommandBase
    {
        public SettingsCommand() : base("settings", "Сгенерировать блок настроек")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("file", "str", true, null, "XML-описание настроек (относительно проекта)"),
                new CommandParameter("out", "str", true, null, "Файл результата"),
                new CommandParameter("format", null, false, "hex", "hex | bin"),
                new CommandParameter("offset", null, false, "0", "Смещение (0x... или dec)"),
                new CommandParameter("set", null, false, null, "Переопределения полей: name=value;name2=value2")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var File = Ctx.Args.Get("file");
            var Out = Ctx.Args.Get("out");

            if (string.IsNullOrEmpty(File)) return CommandResult.Error("settings: параметр 'file' обязателен");
            if (string.IsNullOrEmpty(Out)) return CommandResult.Error("settings: параметр 'out' обязателен");

            var FilePath = ResolveFile(Ctx, Expand(Ctx, File));
            var OutPath = ResolveFile(Ctx, Expand(Ctx, Out));

            string Text;
            try { Text = Ctx.Host.Files.ReadAllText(FilePath); }
            catch (Exception E) { return CommandResult.Error($"settings: не прочитан '{FilePath}': {E.Message}"); }

            List<Field> Fields;
            try { Fields = SettingsLoader.Load(Text); }
            catch (Exception E) { return CommandResult.Error($"settings: ошибка разбора '{FilePath}': {E.Message}"); }

            var Overrides = ParseOverrides(Ctx.Args.Get("set"));
            var Data = SettingsBuilder.Build(
                Fields,
                N => Ctx.Host.ResolveArgument(N, Ctx.Args),
                Overrides,
                Src =>
                {
                    try { return Ctx.Host.Files.ReadAllBytes(ResolveFile(Ctx, Src)); }
                    catch { return null; }
                });

            var Format = (Ctx.Args.Get("format") ?? "hex").Trim().ToLowerInvariant();
            var Offset = ParseOffset(Ctx.Args.Get("offset"));

            try
            {
                if ((Format == "bin") || (Format == "binary"))
                    Ctx.Host.Files.WriteAllBytes(OutPath, IntelHex.Binary(Data, Offset));
                else
                    Ctx.Host.Files.WriteAllText(OutPath, IntelHex.Build(Data, Offset));
            }
            catch (Exception E)
            {
                return CommandResult.Error($"settings: не записан '{OutPath}': {E.Message}");
            }

            var Outputs = new ArgumentList();
            Outputs.Set("file", OutPath);
            Outputs.Set("size", Data.Length.ToString());
            Outputs.Set("offset", "0x" + Offset.ToString("X8"));

            return new CommandResult(CommandStatus.Ok, "", Outputs);
        }

        private static Dictionary<string, string> ParseOverrides(string Spec)
        {
            var Res = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(Spec)) return Res;

            foreach (var Part in Spec.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var P = Part.Trim();
                var Eq = P.IndexOf('=');
                if (Eq <= 0) continue;

                Res[P.Substring(0, Eq).Trim()] = P.Substring(Eq + 1).Trim();
            }

            return Res;
        }

        /// <summary>Раскрыть {переменные} в значении (для нативных команд движок это не делает).</summary>
        private static string Expand(CommandContext Ctx, string V)
        {
            if (string.IsNullOrEmpty(V) || (V.IndexOf('{') < 0)) return V;

            try
            {
                return CommandTemplating.Process(V, N => Ctx.Host.ResolveArgument(N, Ctx.Args) ?? ("{" + N + "}"));
            }
            catch { return V; }
        }

        private static string ResolveFile(CommandContext Ctx, string File)
        {
            if (Path.IsPathRooted(File)) return File;
            return Path.Combine(Ctx.WorkingDir ?? "", File);
        }

        private static int ParseOffset(string Text)
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
    }
}
