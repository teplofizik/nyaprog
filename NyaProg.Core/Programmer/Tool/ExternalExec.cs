using System;
using System.Collections.Generic;
using System.IO;

using Programmer.Modules;
using Programmer.Options.Rewrite;
using Programmer.Platform;
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
        /// Выполнить внешнее ПО через переданный раннер процессов.
        /// </summary>
        public static ProcessResult Exec(string Program, string ProgArgs, ArgumentList Args,
                                         OptRewrite[] Rewrite, IProcessRunner Process)
        {
            var CmdPath = ProcessCommand(Program, Args, Rewrite);
            var CmdArgs = ProcessCommand(ProgArgs, Args, Rewrite);

            string Dir = Args.Get("$dir");

            return Process.Run(CmdPath, CmdArgs, Dir);
        }
    }
}
