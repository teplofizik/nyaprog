using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Options.Rewrite;

namespace Programmer.Tool.InternalTools
{
    class TlBase : UniTool
    {
        protected string[] Commands = new string[] { };

        public TlBase(string Name, string[] Commands) : base(Name)
        {
            this.Commands = Commands;
        }

        /// <summary>
        /// Внутренние инструменты по умолчанию доступны
        /// </summary>
        /// <returns></returns>
        public override bool Avail() => true;

        /// <summary>
        /// Можно ли использовать этот набор команд
        /// </summary>
        /// <returns></returns>
        public override bool Valid() => true;

        /// <summary>
        /// Опции
        /// </summary>
        public override Options.Option[] GetOptions() => new Options.Option[] { };

        /// <summary>
        /// Поддерживается ли команда этим набором
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public override bool CmdAvail(string Command) => Commands.Contains(Command);

        /// <summary>
        /// Выполнить команду с стандартными запросами
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Dir"></param>
        /// <param name="Option"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        protected virtual ExecResult Exec(string Command, string Dir, string Option, ArgumentList Args, OptRewrite[] Rewrite) => new ERError("No command available");

        /// <summary>
        /// Выполнить команду (с набором аргументов)
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public override ExecResult Exec(string Command, ArgumentList Args, OptRewrite[] Rewrite)
        {
            string Dir = Args.Get("$dir");
            string Option = Args.Get("value");

            return Exec(Command, Dir, Option, Args, Rewrite);
        }
    }
}
