using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Modules;
using Programmer.Platform;
using Programmer.Tool.XMLTools.XML;
using Programmer.Options.Rewrite;

namespace Programmer.Tool.XMLTools
{
    class TlBaseXML : UniTool
    {
        XMLTool T = new XMLTool();

        public TlBaseXML(string Filename, bool Custom) : base("xml")
        {
            XMLLoader.Load(Filename, T, Custom);

            Name = T.ToolName;
            foreach (var A in T.ControllerTypes) Arch.Add(A);
            foreach (var T in T.Tags) Tags.Add(T);
        }

        /// <summary>
        /// Можно ли использовать этот набор команд
        /// </summary>
        /// <returns></returns>
        public override bool Valid() => File.Exists(T.ToolPath);

        /// <summary>
        /// Проверки нет пока
        /// </summary>
        /// <returns></returns>
        public override bool Avail() => true;
        
        /// <summary>
        /// Опции
        /// </summary>
        public override Options.Option[] GetOptions() => T.Options.ToArray();

        /// <summary>
        /// Поддерживается ли команда этим набором
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public override bool CmdAvail(string Command) => T.CmdAvail(Command);

        /// <summary>
        /// Список поддерживаемых команд.
        /// </summary>
        public override string[] GetCommands() => T.GetCommands();

        /// <summary>
        /// Схема параметров команды.
        /// </summary>
        public override CommandParameter[] GetCommandParameters(string Command) => T.GetCommandParameters(Command);

        /// <summary>
        /// Выполнить команду (с набором аргументов)
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public override ExecResult Exec(string Command, ArgumentList Args, OptRewrite[] Rewrite, IProcessRunner Process) => T.Exec(Command, Args, Rewrite, Process);
    }
}
