using System;
using Programmer.Environment;

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

            return 0;
        }
    }
}
