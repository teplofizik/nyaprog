using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Environment;

namespace Programmer.Tool
{
    public class Executer
    {
        /// <summary>
        /// Выполнить команду
        /// </summary>
        /// <param name="Tool">Выбранный программатор</param>
        /// <param name="UserTools">Набор команд, не связанных с программатором</param>
        /// <param name="Command">Команда из скрипта</param>
        /// <param name="Args">Набор параметров для команды (аргументы)</param>
        /// <returns></returns>
        public ExecResult Exec(ProgEnv E, UniTool Tool, ToolSet UserTools, string Command, ArgumentList Args)
        {
            if ((Tool != null) && (Tool.CmdAvail(Command)))
                return Tool.Exec(Command, Args, E.OptRewriteRules);
            else
            {
                foreach(var T in UserTools.Tools)
                {
                    if(T.CmdAvail(Command))
                        return T.Exec(Command, Args, E.OptRewriteRules);
                }
            }

            return new ERError("No such command");
        }
    }
}
