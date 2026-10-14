using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Extension.Argument;
using Programmer.Modules;
using Programmer.Options.Rewrite;
using Programmer.Platform;

namespace Programmer.Tool.XMLTools.XML
{
    public class XMLTool
    {
        /// <summary>
        /// Список поддерживаемых архитектур
        /// </summary>
        public List<string> ControllerTypes = new List<string>();

        /// <summary>
        /// Список тегов
        /// </summary>
        public List<string> Tags = new List<string>();

        /// <summary>
        /// Название программатора
        /// </summary>
        public string ToolName = "";

        /// <summary>
        /// Интерпретатор (python, mono и т.д.)
        /// </summary>
        public string Interpreter = "";

        /// <summary>
        /// Путь к программе
        /// </summary>
        public string ToolPath = "";

        /// <summary>
        /// Поддерживаемые действия
        /// </summary>
        public List<XMLToolAction> Actions = new List<XMLToolAction>();

        /// <summary>
        /// Опции
        /// </summary>
        public List<Options.Option> Options = new List<Options.Option>();
        
        private string GetMessage(ProcessResult Result, List<XMLResultDetector> Mask)
        {
            foreach (var M in Mask)
            {
                var R = M.Get(Result);
                if(R != null) return R;
            }
            return null;
        }
        
        public bool CmdAvail(string Command) => Actions.Exists(A => (A.Name == Command));

        /// <summary>
        /// Список поддерживаемых команд.
        /// </summary>
        public string[] GetCommands() => Actions.Select(A => A.Name).ToArray();

        /// <summary>
        /// Схема параметров команды (из блока params описания).
        /// </summary>
        public CommandParameter[] GetCommandParameters(string Command)
        {
            var A = GetAction(Command);
            if (A == null) return new CommandParameter[0];

            var Res = new List<CommandParameter>();
            foreach (var K in A.Defaults.Keys)
                Res.Add(new CommandParameter(K, null, false, A.Defaults.Get(K)));

            return Res.ToArray();
        }

        private XMLToolAction GetAction(string Command)
        {
            foreach(var A in Actions)
            {
                if (A.Name == Command)
                    return A;
            }

            return null;
        }

        /// <summary>
        /// Выполнить команду
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <param name="Rewrite"></param>
        /// <returns></returns>
        public ExecResult Exec(string Command, ArgumentList Args, OptRewrite[] Rewrite, IProcessRunner Process)
        {
            var A = GetAction(Command);
            if (A == null)
                return new ERError($"No available command '{Command}'");

            string P = (A.CustomToolPath != null) ? A.CustomToolPath : ToolPath;
            if (!File.Exists(P)) return new ERError($"No specified path for tool: {P}");

            ProcessResult Result;
            try
            {
                Result = (Interpreter.Length > 0)
                       ? ExternalExec.Exec(Interpreter, $"{P} {A.Command}", Args, Rewrite, Process)
                       : ExternalExec.Exec(P, A.Command, Args, Rewrite, Process);
            }
            catch (Exception Exc)
            {
                return new ERError(Exc.Message);
            }

            if (Result == null) return new ERError("No output");
            if (Result.ExitCode != 0) return new ERError($"Process exited with code {Result.ExitCode}");

            var E = GetMessage(Result, A.ErrorMask);
            if (E != null) return new ERError(E);

            var W = GetMessage(Result, A.WarningMask);

            ExecResult Res = (W != null) ? (ExecResult)new ERWarning(W) : new EROk();
            CollectOutputs(A, Result, Res.Outputs);
            return Res;
        }

        /// <summary>
        /// Извлечь выходные переменные из вывода утилиты.
        /// </summary>
        private static void CollectOutputs(XMLToolAction A, ProcessResult R, ArgumentList Outputs)
        {
            foreach (var O in A.Outputs)
            {
                if (string.IsNullOrEmpty(O.Name) || string.IsNullOrEmpty(O.Pattern)) continue;

                var M = Regex.Match(R.Output ?? "", O.Pattern, RegexOptions.IgnoreCase);
                if (M.Success && (M.Groups.Count > O.Group))
                    Outputs.Set(O.Name, M.Groups[O.Group].Value);
            }
        }
    }
}
