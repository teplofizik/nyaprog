using System;
using System.IO;

using Programmer.Modules;
using Programmer.Platform;

namespace Programmer.Modules.Internal
{
    /// <summary>
    /// Файловые операции (copy/copyto/delete) на нативных командах.
    /// </summary>
    public sealed class FileOpsTool : CodedToolBase
    {
        public FileOpsTool() : base("core.fileops", "FileTools")
        {
            AddCommand(new CopyCommand());
            AddCommand(new CopyToCommand());
            AddCommand(new DeleteCommand());
        }
    }

    abstract class FileCommand : CommandBase
    {
        protected FileCommand(string Id, string Title) : base(Id, Title) { }

        protected static string Source(CommandContext Ctx) => Ctx.Args.Get("value") ?? "";

        protected static string Dir(CommandContext Ctx) => Ctx.WorkingDir ?? Ctx.Args.Get("$dir") ?? "";

        /// <summary>Итоговый путь в проекте (Dir + имя файла).</summary>
        protected static string InProject(CommandContext Ctx, string File) =>
            Path.Combine(Dir(Ctx), Path.GetFileName(File));

        protected static bool CompareFile(IFileSystem FS, string A, string B)
        {
            if (!FS.Exists(A) || !FS.Exists(B)) return false;

            if (FS.GetLength(A) != FS.GetLength(B)) return false;
            if (FS.GetLastWriteTimeUtc(A).CompareTo(FS.GetLastWriteTimeUtc(B)) != 0) return false;

            return true;
        }
    }

    sealed class CopyCommand : FileCommand
    {
        public CopyCommand() : base("copy", "Копировать в проект") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var FS = Ctx.Host.Files;
            var Option = Source(Ctx);

            if (!FS.Exists(Option))
                return CommandResult.Error("No source file");

            var NewFn = InProject(Ctx, Option);

            try
            {
                if (CompareFile(FS, NewFn, Option))
                    return CommandResult.Warning("Files are equal");

                FS.Copy(Option, NewFn, true);
                return CommandResult.Ok();
            }
            catch (Exception E)
            {
                return CommandResult.Error(E.Message);
            }
        }
    }

    sealed class CopyToCommand : FileCommand
    {
        public CopyToCommand() : base("copyto", "Копировать из проекта") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var FS = Ctx.Host.Files;
            var Option = Source(Ctx);

            if (!FS.Exists(Option))
                return CommandResult.Error("No source file");

            var NewFn = InProject(Ctx, Option);

            try
            {
                if (CompareFile(FS, NewFn, Option))
                    return CommandResult.Warning("Files are equal");

                FS.Copy(NewFn, Option, true);
                return CommandResult.Ok();
            }
            catch (Exception E)
            {
                return CommandResult.Error(E.Message);
            }
        }
    }

    sealed class DeleteCommand : FileCommand
    {
        public DeleteCommand() : base("delete", "Удалить файл") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var FS = Ctx.Host.Files;
            var Option = Source(Ctx);

            if (!FS.Exists(Option))
                return CommandResult.Ok();

            try
            {
                FS.Delete(Option);
                return CommandResult.Ok();
            }
            catch (Exception E)
            {
                return CommandResult.Error($"Could not delete file: {Option} ({E.Message})");
            }
        }
    }
}
