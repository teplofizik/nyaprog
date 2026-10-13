using System;
using System.IO;
using System.Linq;

using Extension.Argument;
using NyaProg.Hosting;
using Programmer.Environment;
using Programmer.Modules;
using Programmer.Project;
using Programmer.Tool;

using PAction = Programmer.Project.Action;

namespace NyaProg.Cli
{
    /// <summary>
    /// Headless-точка входа поверх NyaProg.Core.
    ///   NyaProg.Cli [config.xml]                       — сводка окружения и реестра
    ///   NyaProg.Cli &lt;config&gt; --exec &lt;cmd&gt; [value]    — выполнить команду (утилиты/встроенные)
    /// </summary>
    static class Program
    {
        static int Main(string[] args)
        {
            try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }

            var ConfigFile = (args.Length > 0) ? args[0] : "config.xml";

            // Лог из config нужен до загрузки окружения, чтобы предупреждения попали в него.
            var Conf = new Config();
            Conf.Load(ConfigFile);
            Programmer.Log.LogFileName = Conf.Log;

            var Env = new ProgEnv();
            Env.Load(ConfigFile);
            PluginHost.Load(Env, Env.Conf.Plugins);

            if ((args.Length >= 3) && (args[1] == "--exec"))
                return ExecCommand(Env, args[2], args.Skip(3).ToArray());

            Dump(Env);
            return 0;
        }

        static int ExecCommand(ProgEnv Env, string Command, string[] CmdArgs)
        {
            var SE = new ScriptExec();
            var P = new Project { Dir = Directory.GetCurrentDirectory() };
            var S = new Script();
            var A = new PAction(Command);

            foreach (var Arg in CmdArgs)
            {
                var Eq = Arg.IndexOf('=');
                if (Eq > 0)
                    A.Arguments.Set(Arg.Substring(0, Eq), Arg.Substring(Eq + 1));
                else
                    A.Arguments.Set("value", Arg);
            }

            var Options = new ArgumentList();
            var R = SE.Exec(Env, null, P, S, A, Options);

            Console.WriteLine($"exec '{Command}': {R.Result} {R.Message}");
            foreach (var K in Options.Keys)
                Console.WriteLine($"  var {K} = {Options.Get(K)}");

            return (R.Result == ExecResultType.Error) ? 1 : 0;
        }

        static void Dump(ProgEnv Env)
        {
            Console.WriteLine($"NyaProg.Cli. Tools: {Env.Tools.Length}");
            foreach (var T in Env.Tools)
                Console.WriteLine($"  - {T.Name} [{string.Join(", ", T.GetSupportedArch())}]");

            Console.WriteLine($"Projects: {Env.Projects.Length}");
            foreach (var P in Env.Projects)
            {
                Console.WriteLine($"  * {P.Name} ({P.Scripts.Count} scripts)");
                foreach (var S in P.Scripts)
                    Console.WriteLine($"      - {S.Name} ({S.Actions.Count} steps)");
            }

            Console.WriteLine($"Registry tools: {Env.Commands.Tools.Count}");
            foreach (var T in Env.Commands.Tools)
                Console.WriteLine($"  [{T.Kind}] {T.Name}: {string.Join(", ", T.GetCommands().Select(C => C.Id))}");

            // Схемы параметров команд.
            foreach (var T in Env.Commands.Tools)
                foreach (var C in T.GetCommands())
                {
                    if ((C.Descriptor == null) || (C.Descriptor.Parameters == null) || (C.Descriptor.Parameters.Count == 0))
                        continue;

                    Console.WriteLine($"  params {C.Descriptor.Id}:");
                    foreach (var Par in C.Descriptor.Parameters)
                        Console.WriteLine($"    {Par.Name} default={Par.Default ?? "<none>"} required={Par.Required} type={Par.Type ?? "-"}");
                }

            ICommand Cmd;
            foreach (var Name in new[] { "copy", "message", "download", "lookup", "wflash" })
                Console.WriteLine($"  resolve '{Name}': {(Env.Commands.TryResolve(Name, out Cmd) ? Cmd.Descriptor.Id : "<none>")}");
        }
    }
}
