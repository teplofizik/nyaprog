using System;
using System.IO;

using Programmer.Modules;

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

        protected static bool CompareFile(string A, string B)
        {
            if (!File.Exists(A) || !File.Exists(B)) return false;

            var AI = new FileInfo(A);
            var BI = new FileInfo(B);

            if (AI.Length != BI.Length) return false;
            if (AI.LastWriteTimeUtc.CompareTo(BI.LastWriteTimeUtc) != 0) return false;

            return true;
        }
    }

    sealed class CopyCommand : FileCommand
    {
        public CopyCommand() : base("copy", "Копировать в проект") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Option = Source(Ctx);

            if (!File.Exists(Option))
                return CommandResult.Error("No source file");

            var NewFn = Dir(Ctx) + Path.GetFileName(Option);

            try
            {
                if (CompareFile(NewFn, Option))
                    return CommandResult.Warning("Files are equal");

                File.Delete(NewFn);
                File.Copy(Option, NewFn);
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
            var Option = Source(Ctx);

            if (!File.Exists(Option))
                return CommandResult.Error("No source file");

            var NewFn = Dir(Ctx) + Path.GetFileName(Option);

            try
            {
                if (CompareFile(NewFn, Option))
                    return CommandResult.Warning("Files are equal");

                File.Delete(Option);
                File.Copy(NewFn, Option);
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
            var Option = Source(Ctx);

            if (!File.Exists(Option))
                return CommandResult.Ok();

            try
            {
                File.Delete(Option);
                return CommandResult.Ok();
            }
            catch (Exception E)
            {
                return CommandResult.Error($"Could not delete file: {Option} ({E.Message})");
            }
        }
    }
}
