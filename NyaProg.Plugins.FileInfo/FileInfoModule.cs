using System;
using System.Collections.Generic;
using System.IO;

using Extension.Argument;
using Programmer.Modules;

namespace NyaProg.Plugins.FileInfo
{
    /// <summary>
    /// Плагин сведений о файлах: размер файла в переменную (одну или несколько).
    /// </summary>
    public sealed class FileInfoModule : ModuleBase
    {
        public override string Id => "fileinfo";

        public override string Name => "File info";

        public override IEnumerable<ITool> GetTools() => new ITool[] { new FileInfoTool() };
    }

    public sealed class FileInfoTool : CodedToolBase
    {
        public FileInfoTool() : base("fileinfo.tool", "Files", ToolKind.Internal)
        {
            AddCommand(new FileSizeCommand());
        }
    }

    /// <summary>
    /// filesize: размер файла(ов) в переменные окружения.
    ///   file=<путь> var=<имя>          — один файл (по умолчанию var=size)
    ///   set="name=path;name2=path2"    — несколько
    /// </summary>
    sealed class FileSizeCommand : CommandBase
    {
        public FileSizeCommand() : base("filesize", "Размер файла в переменную")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("file", null, false, null, "Файл (относительно каталога проекта)"),
                new CommandParameter("var", null, false, "size", "Имя переменной"),
                new CommandParameter("set", null, false, null, "Несколько: name=path;name2=path2")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Outputs = new ArgumentList();

            var File = Ctx.Args.Get("file");
            var Var = Ctx.Args.Get("var") ?? "size";
            if (!string.IsNullOrEmpty(File))
                Outputs.Set(Var, Size(Ctx, File));

            var Set = Ctx.Args.Get("set");
            if (!string.IsNullOrEmpty(Set))
            {
                foreach (var Part in Set.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var Eq = Part.IndexOf('=');
                    if (Eq <= 0) continue;

                    var Name = Part.Substring(0, Eq).Trim();
                    var Path = Part.Substring(Eq + 1).Trim();
                    if (Name.Length == 0) continue;

                    Outputs.Set(Name, Size(Ctx, Path));
                }
            }

            return new CommandResult(CommandStatus.Ok, "", Outputs);
        }

        private static string Size(CommandContext Ctx, string File)
        {
            try
            {
                var P = Path.IsPathRooted(File) ? File : Path.Combine(Ctx.WorkingDir ?? "", File);
                return Ctx.Host.Files.GetLength(P).ToString();
            }
            catch { return "0"; }
        }
    }
}
