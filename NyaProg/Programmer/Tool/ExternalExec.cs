using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Programmer.Options.Rewrite;
using Extension.Argument;

namespace Programmer.Tool
{
    static class ExternalExec
    {
        private static string[] GetParams(string Command)
        {
            var Match = Regex.Match(Command, @"\{([^};]*)\}");
            var Res = new List<string>();

            for (int i = 1; i < Match.Groups.Count; i++)
            {
                var G = Match.Groups[i].Value;

                Res.Add(G);
            }
            return Res.ToArray();
        }

        /// <summary>
        /// Обработка командной строки
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        private static string ProcessCommand(string Command, ArgumentList Args, OptRewrite[] Rewrite)
        {
            var Params = GetParams(Command);

            while (Params.Length > 0)
            {
                foreach (var K in Params)
                {
                    var Res = Args.Get(K);
                    if (Res == null)
                    {
                        // Такого параметра нет, проверим переназначение
                        foreach (var R in Rewrite)
                        {
                            if (R.Check(K))
                            {
                                Res = R.GetValue(K, Args);
                                break;
                            }
                        }
                    }
                    if (Res == null)
                    {
                        // Увы, но заменять не на что, генерируем исключение
                        throw new InvalidOperationException($"no argument for param {{{K}}}: {Command}");
                    }
                    Command = Command.Replace($"{{{K}}}", Res);
                }

                Params = GetParams(Command);
            }

            return Command;
        }
        
        /// <summary>
        /// Выполнить внешнее ПО
        /// </summary>
        /// <param name="Program"></param>
        /// <param name="ProgArgs"></param>
        /// <param name="Args"></param>
        /// <param name="Rewrite"></param>
        /// <returns></returns>
        public static ExternalToolResult Exec(string Program, string ProgArgs, ArgumentList Args, OptRewrite[] Rewrite)
        {
            var CmdPath = ProcessCommand(Program, Args, Rewrite);
            var CmdArgs = ProcessCommand(ProgArgs, Args, Rewrite);

            string Dir = Args.Get("$dir");
            
            return ExternalTool.Run(CmdPath, CmdArgs, Dir);
        }
    }
}
