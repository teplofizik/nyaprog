using System;
using System.Collections.Generic;
using System.IO;

using Extension.Argument;
using Programmer.Modules;
using Programmer.Platform;

namespace NyaProg.Plugins.Temp
{
    /// <summary>
    /// Плагин временных файлов: выдаёт путь во временной папке в переменную,
    /// чтобы промежуточные файлы (hex/bin/суммы) не попадали в каталог проекта.
    /// </summary>
    public sealed class TempModule : ModuleBase
    {
        public override string Id => "temp";

        public override string Name => "Temp files";

        public override IEnumerable<ITool> GetTools() => new ITool[] { new TempTool() };
    }

    public sealed class TempTool : CodedToolBase
    {
        public TempTool() : base("temp.tool", "Temp", ToolKind.Internal)
        {
            AddCommand(new TempFileCommand());
            AddCommand(new TempDirCommand());
            AddCommand(new DelFileCommand());
            AddCommand(new DelDirCommand());
            AddCommand(new TempCleanCommand());
            AddCommand(new TempInfoCommand());
        }
    }

    static class TempPaths
    {
        public static string Root
        {
            get
            {
                var R = Path.Combine(Path.GetTempPath(), "nyaprog");
                Directory.CreateDirectory(R);
                return R;
            }
        }

        public static string Unique(string Prefix)
        {
            var Guid8 = Guid.NewGuid().ToString("N").Substring(0, 8);
            return Prefix + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + Guid8;
        }

        public static bool True(string Text)
        {
            if (string.IsNullOrEmpty(Text)) return false;
            Text = Text.Trim().ToLowerInvariant();
            return (Text == "1") || (Text == "true") || (Text == "yes") || (Text == "on") || (Text == "да");
        }

        public static string Resolve(CommandContext Ctx, string File)
        {
            if (Path.IsPathRooted(File)) return File;
            return Path.Combine(Ctx.WorkingDir ?? "", File);
        }
    }

    /// <summary>tempclean: удалить зарегистрированные временные файлы сейчас.</summary>
    sealed class TempCleanCommand : CommandBase
    {
        public TempCleanCommand() : base("tempclean", "Убрать временные файлы")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("all", null, false, null, "Удалить всё в корне temp (1/true)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Outputs = new ArgumentList();

            if (TempPaths.True(Ctx.Args.Get("all")))
            {
                var Root = TempPaths.Root;
                var N = 0;

                foreach (var F in Directory.GetFiles(Root, "*", SearchOption.AllDirectories)) { TempFiles.Delete(F); N++; }
                foreach (var D in Directory.GetDirectories(Root, "*", SearchOption.AllDirectories)) { TempFiles.Delete(D); N++; }

                Outputs.Set(TempFiles.ListKey, "");
                return new CommandResult(CommandStatus.Ok, $"tempclean: удалено {N}", Outputs);
            }

            var Count = TempFiles.Cleanup(Ctx.Args);
            Outputs.Set(TempFiles.ListKey, "");

            return new CommandResult(CommandStatus.Ok, $"tempclean: удалено {Count}", Outputs);
        }
    }

    /// <summary>tempinfo: диагностика корня temp.</summary>
    sealed class TempInfoCommand : CommandBase
    {
        public TempInfoCommand() : base("tempinfo", "Инфо о temp")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("name", null, false, null, "Переменная для корня temp")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Root = TempPaths.Root;

            var Outputs = new ArgumentList();
            var Name = Ctx.Args.Get("name");
            if (!string.IsNullOrEmpty(Name)) Outputs.Set(Name, Root);

            Outputs.Set("root", Root);
            Outputs.Set("gettemp", Path.GetTempPath());
            Outputs.Set("env_TMP", Environment.GetEnvironmentVariable("TMP") ?? "<null>");
            Outputs.Set("env_TEMP", Environment.GetEnvironmentVariable("TEMP") ?? "<null>");
            Outputs.Set("cwd", Directory.GetCurrentDirectory());
            Outputs.Set("work", Ctx.WorkingDir ?? "<null>");

            return new CommandResult(CommandStatus.Ok, "", Outputs);
        }
    }

    /// <summary>tempfile: путь временного файла в переменную.</summary>
    sealed class TempFileCommand : CommandBase
    {
        public TempFileCommand() : base("tempfile", "Временный файл")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("name", "str", true, null, "Имя переменной для пути"),
                new CommandParameter("ext", null, false, "tmp", "Расширение (без точки)"),
                new CommandParameter("subdir", null, false, null, "Подкаталог под temp"),
                new CommandParameter("prefix", null, false, "nyap_", "Префикс имени"),
                new CommandParameter("create", null, false, null, "Создать пустой файл (1/true)"),
                new CommandParameter("keep", null, false, null, "Не удалять в конце прогона (1/true)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Name = Ctx.Args.Get("name");
            if (string.IsNullOrEmpty(Name)) return CommandResult.Error("tempfile: параметр 'name' обязателен");

            var Dir = TempPaths.Root;
            var Sub = Ctx.Args.Get("subdir");
            if (!string.IsNullOrEmpty(Sub)) Dir = Path.Combine(Dir, Sub);
            Directory.CreateDirectory(Dir);

            var Ext = (Ctx.Args.Get("ext") ?? "tmp").Trim().TrimStart('.');
            var Prefix = Ctx.Args.Get("prefix");
            if (string.IsNullOrEmpty(Prefix)) Prefix = "nyap_";

            var File = Path.Combine(Dir, TempPaths.Unique(Prefix) + (Ext.Length > 0 ? "." + Ext : ""));

            if (TempPaths.True(Ctx.Args.Get("create")))
            {
                try { using (var S = System.IO.File.Create(File)) { } }
                catch (Exception E) { return CommandResult.Error($"tempfile: не создан '{File}': {E.Message}"); }
            }

            var Outputs = new ArgumentList();
            Outputs.Set(Name, File);

            if (!TempPaths.True(Ctx.Args.Get("keep")))
                Outputs.Set(TempFiles.ListKey, TempFiles.Append(Ctx.Args.Get(TempFiles.ListKey), File));

            return new CommandResult(CommandStatus.Ok, File, Outputs);
        }
    }

    /// <summary>tempdir: путь временного каталога в переменную.</summary>
    sealed class TempDirCommand : CommandBase
    {
        public TempDirCommand() : base("tempdir", "Временный каталог")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("name", "str", true, null, "Имя переменной для пути"),
                new CommandParameter("prefix", null, false, "nyap_", "Префикс имени"),
                new CommandParameter("keep", null, false, null, "Не удалять в конце прогона (1/true)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Name = Ctx.Args.Get("name");
            if (string.IsNullOrEmpty(Name)) return CommandResult.Error("tempdir: параметр 'name' обязателен");

            var Prefix = Ctx.Args.Get("prefix");
            if (string.IsNullOrEmpty(Prefix)) Prefix = "nyap_";

            var Dir = Path.Combine(TempPaths.Root, TempPaths.Unique(Prefix));
            try { Directory.CreateDirectory(Dir); }
            catch (Exception E) { return CommandResult.Error($"tempdir: не создан '{Dir}': {E.Message}"); }

            var Outputs = new ArgumentList();
            Outputs.Set(Name, Dir);

            if (!TempPaths.True(Ctx.Args.Get("keep")))
                Outputs.Set(TempFiles.ListKey, TempFiles.Append(Ctx.Args.Get(TempFiles.ListKey), Dir));

            return new CommandResult(CommandStatus.Ok, Dir, Outputs);
        }
    }

    /// <summary>delfile: удалить файл.</summary>
    sealed class DelFileCommand : CommandBase
    {
        public DelFileCommand() : base("delfile", "Удалить файл")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("path", "str", true, null, "Файл"),
                new CommandParameter("ok", null, false, null, "Не ошибка, если файла нет (1/true)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Arg = Ctx.Args.Get("path");
            if (string.IsNullOrEmpty(Arg)) return CommandResult.Error("delfile: параметр 'path' обязателен");

            var File = TempPaths.Resolve(Ctx, Arg);
            if (!System.IO.File.Exists(File))
            {
                if (TempPaths.True(Ctx.Args.Get("ok"))) return CommandResult.Ok();
                return CommandResult.Error($"delfile: файл не найден '{File}'");
            }

            try { System.IO.File.Delete(File); }
            catch (Exception E) { return CommandResult.Error($"delfile: не удалён '{File}': {E.Message}"); }

            return new CommandResult(CommandStatus.Ok, File, null);
        }
    }

    /// <summary>deldir: удалить каталог (рекурсивно).</summary>
    sealed class DelDirCommand : CommandBase
    {
        public DelDirCommand() : base("deldir", "Удалить каталог")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("path", "str", true, null, "Каталог"),
                new CommandParameter("ok", null, false, null, "Не ошибка, если каталога нет (1/true)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Arg = Ctx.Args.Get("path");
            if (string.IsNullOrEmpty(Arg)) return CommandResult.Error("deldir: параметр 'path' обязателен");

            var Dir = TempPaths.Resolve(Ctx, Arg);
            if (!Directory.Exists(Dir))
            {
                if (TempPaths.True(Ctx.Args.Get("ok"))) return CommandResult.Ok();
                return CommandResult.Error($"deldir: каталог не найден '{Dir}'");
            }

            try { Directory.Delete(Dir, true); }
            catch (Exception E) { return CommandResult.Error($"deldir: не удалён '{Dir}': {E.Message}"); }

            return new CommandResult(CommandStatus.Ok, Dir, null);
        }
    }
}
