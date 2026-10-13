using System.Collections.Generic;

using Extension.Argument;
using Programmer.Modules;

namespace Programmer.Modules.Net
{
    /// <summary>
    /// lookup: получить значения по URL и разложить их в переменные.
    /// Параметры: url (обязателен), method, headers, extract.
    /// </summary>
    sealed class LookupCommand : CommandBase
    {
        public LookupCommand() : base("lookup", "Получить значения по URL")
        {
            Descriptor.Parameters = new List<CommandParameter>
            {
                new CommandParameter("url", "str", true, null, "URL (шаблон с {переменными})"),
                new CommandParameter("method", null, false, "GET", "HTTP-метод"),
                new CommandParameter("headers", null, false, null, "Заголовки: A: B; C: D"),
                new CommandParameter("extract", null, false, null,
                    "Правила: name=json:$.path; name2=regex:pattern; name3=text: (по умолчанию body)")
            };
        }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Url = Ctx.Args.Get("url");
            if (string.IsNullOrEmpty(Url)) return CommandResult.Error("lookup: url is required");

            Url = HttpArgs.ExpandUrl(Ctx, Url);
            var Method = Ctx.Args.Get("method") ?? "GET";

            var Body = Ctx.Host.Http.GetText(Url, Method, HttpArgs.ParseHeaders(Ctx.Args.Get("headers")));
            if (Body == null) return CommandResult.Error($"lookup: request failed: {Url}");

            var Outputs = new ArgumentList();
            Extraction.Apply(Ctx.Args.Get("extract"), Body, Outputs);

            return new CommandResult(CommandStatus.Ok, "", Outputs);
        }
    }
}
