using System;
using System.Collections.Generic;
using System.IO;

using Programmer.Modules;
using Programmer.Options.Rewrite;
using Extension.Argument;

namespace Programmer.Tool
{
    static class ExternalExec
    {
        /// <summary>
        /// Обработка командной строки: подстановка {name} с учётом правил перезаписи.
        /// </summary>
        private static string ProcessCommand(string Command, ArgumentList Args, OptRewrite[] Rewrite)
        {
            return CommandTemplating.Process(Command, K =>
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

                return Res;
            });
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
