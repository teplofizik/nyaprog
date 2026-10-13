using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Options;
using Programmer.Options.Rewrite;
using Programmer.Tool;

namespace Programmer.Modules
{
    /// <summary>
    /// Адаптер: оборачивает legacy-инструмент (<see cref="UniTool"/>) в <see cref="ITool"/>,
    /// чтобы существующие .etool и встроенные утилиты участвовали в новом реестре команд.
    /// </summary>
    sealed class LegacyToolModule : ITool
    {
        private readonly UniTool Tool;
        private readonly OptRewrite[] Rewrite;
        private readonly ICommand[] Cmds;

        public ToolKind Kind { get; }

        public LegacyToolModule(UniTool Tool, ToolKind Kind, OptRewrite[] Rewrite)
        {
            this.Tool = Tool;
            this.Kind = Kind;
            this.Rewrite = Rewrite ?? new OptRewrite[0];

            Id = $"legacy:{Tool.Name}";
            Cmds = Tool.GetCommands()
                       .Select(N => (ICommand)new LegacyCommand(this, N))
                       .ToArray();
        }

        public string Id { get; }

        public string Name => Tool.Name;

        public string[] SupportedArch => Tool.GetSupportedArch();

        public string[] Tags => Tool.GetTags();

        public Option[] GetOptions() => Tool.GetOptions();

        public ICommand[] GetCommands() => Cmds;

        public bool SupportsArch(string[] Arch) => Tool.IsSupportedArch(Arch);

        internal ExecResult Exec(string Command, ArgumentList Args) => Tool.Exec(Command, Args, Rewrite);
    }

    /// <summary>
    /// Legacy-команда: делегирует выполнение исходному <see cref="UniTool.Exec"/>.
    /// Идентификатор команды — её простое имя (как в .nyap-скриптах).
    /// </summary>
    sealed class LegacyCommand : ICommand
    {
        private readonly LegacyToolModule Module;

        public LegacyCommand(LegacyToolModule Module, string Name)
        {
            this.Module = Module;
            Id = Name;
            Descriptor = new CommandDescriptor($"{Module.Id}.{Name}") { Title = Name };
        }

        public string Id { get; }

        public CommandDescriptor Descriptor { get; }

        public CommandResult Execute(CommandContext Ctx)
        {
            var R = Module.Exec(Id, Ctx.Args);

            switch (R.Result)
            {
                case ExecResultType.Ok: return CommandResult.Ok(R.Message);
                case ExecResultType.Warning: return CommandResult.Warning(R.Message);
                default: return CommandResult.Error(R.Message);
            }
        }
    }
}
