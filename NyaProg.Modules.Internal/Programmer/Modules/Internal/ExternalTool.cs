using System;

using Programmer.Modules;

namespace Programmer.Modules.Internal
{
    /// <summary>
    /// Запуск внешней программы (команда external) через IProcessRunner.
    /// </summary>
    public sealed class ExternalTool : CodedToolBase
    {
        public ExternalTool() : base("core.external", "External")
        {
            AddCommand(new ExternalCommand());
        }
    }

    sealed class ExternalCommand : CommandBase
    {
        public ExternalCommand() : base("external", "Внешняя программа") { }

        public override CommandResult Execute(CommandContext Ctx)
        {
            var Option = Ctx.Args.Get("value") ?? "";

            var Separator = Option.IndexOf(".exe");
            if (Separator < 0)
                return CommandResult.Error("Fail to detect .exe");

            var Exe = Option.Substring(0, Separator + 4);
            var ExeArgs = Option.Substring(Separator + 5);

            try
            {
                Func<string, string> Resolve = N => Ctx.Host.ResolveArgument(N, Ctx.Args);

                var Program = CommandTemplating.Process(Exe, Resolve);
                var Args = CommandTemplating.Process(ExeArgs, Resolve);

                var R = Ctx.Host.ProcessRunner.Run(Program, Args, Ctx.WorkingDir);
                if (R == null)
                    return CommandResult.Error("Fail");

                return (R.ExitCode == 0) ? CommandResult.Ok() : CommandResult.Error(R.Output);
            }
            catch (Exception E)
            {
                return CommandResult.Error(E.Message);
            }
        }
    }
}
