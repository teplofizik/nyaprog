using System;
using System.Collections.Generic;
using System.IO;

using Extension.Argument;
using Programmer.Modules;

namespace Programmer.Modules.Net
{
    /// <summary>
    /// download: скачать URL в файл.
    /// Параметры: url (обязателен), file (обязателен), headers.
    /// </summary>
    sealed class DownloadCommand : CommandBase
    {
        public DownloadCommand() : base("download", "Скачать URL в файл")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("url", "str", true, null, "URL (шаблон с {переменными})"),
                new CommandParameter("file", "str", true, null, "Путь назначения (абсолютный или относительно рабочего каталога)"),
                new CommandParameter("headers", null, false, null, "Заголовки: A: B; C: D")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Url = Ctx.Args.Get("url");
            if (string.IsNullOrEmpty(Url)) return CommandResult.Error("download: url is required");

            var File = Ctx.Args.Get("file");
            if (string.IsNullOrEmpty(File)) return CommandResult.Error("download: file is required");

            Url = HttpArgs.ExpandUrl(Ctx, Url);
            var Path = System.IO.Path.IsPathRooted(File)
                     ? File
                     : System.IO.Path.Combine(Ctx.WorkingDir ?? "", File);

            var Data = Ctx.Host.Http.GetBytes(Url, "GET", HttpArgs.ParseHeaders(Ctx.Args.Get("headers")));
            if (Data == null) return CommandResult.Error($"download: request failed: {Url}");

            try
            {
                Ctx.Host.Files.WriteAllBytes(Path, Data);
            }
            catch (Exception E)
            {
                return CommandResult.Error($"download: {E.Message}");
            }

            var Outputs = new ArgumentList();
            Outputs.Set("file", Path);
            Outputs.Set("size", Data.Length.ToString());

            return new CommandResult(CommandStatus.Ok, "", Outputs);
        }
    }
}
