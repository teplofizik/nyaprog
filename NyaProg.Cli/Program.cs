using System;
using System.Linq;
using Programmer.Environment;
using Programmer.Modules;

namespace NyaProg.Cli
{
    /// <summary>
    /// Headless-точка входа: работает поверх NyaProg.Core без UI.
    /// Пока только проверяет загрузку окружения (проекты/инструменты).
    /// </summary>
    static class Program
    {
        static int Main(string[] args)
        {
            try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }

            var ConfigFile = (args.Length > 0) ? args[0] : "config.xml";

            Console.WriteLine($"NyaProg.Cli. Config: {ConfigFile}");

            // Лог из config нужен до загрузки окружения, чтобы предупреждения попали в него.
            var Conf = new Config();
            Conf.Load(ConfigFile);
            Programmer.Log.LogFileName = Conf.Log;

            var Env = new ProgEnv();
            Env.Load(ConfigFile);

            Console.WriteLine($"Tools: {Env.Tools.Length}");
            foreach (var T in Env.Tools)
                Console.WriteLine($"  - {T.Name} [{string.Join(", ", T.GetSupportedArch())}]");

            Console.WriteLine($"Projects: {Env.Projects.Length}");
            foreach (var P in Env.Projects)
            {
                Console.WriteLine($"  * {P.Name} ({P.Scripts.Count} scripts)");
                foreach (var S in P.Scripts)
                    Console.WriteLine($"      - {S.Name} ({S.Actions.Count} steps)");
            }

            // Реестр команд (Фаза 1).
            Console.WriteLine($"Registry tools: {Env.Commands.Tools.Count}");
            foreach (var T in Env.Commands.Tools)
                Console.WriteLine($"  [{T.Kind}] {T.Name}: {string.Join(", ", T.GetCommands().Select(C => C.Id))}");

            ICommand Cmd;
            foreach (var Name in new[] { "copy", "message", "wflash", "bincvt" })
                Console.WriteLine($"  resolve '{Name}': {(Env.Commands.TryResolve(Name, out Cmd) ? Cmd.Descriptor.Id : "<none>")}");

            return 0;
        }
    }
}
