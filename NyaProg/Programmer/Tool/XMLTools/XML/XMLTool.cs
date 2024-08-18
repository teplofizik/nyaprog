using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Options.Rewrite;

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
        
        private string GetMessage(ExternalToolResult Result, List<XMLResultDetector> Mask)
        {
            var T = Result.Result.Split(new char[] { '\n', '\r' });
            
            foreach (var M in Mask)
            {
                var R = M.Get(Result);
                if(R != null) return R;
            }
            return null;
        }
        
        public bool CmdAvail(string Command) => Actions.Exists(A => (A.Name == Command));

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
        public ExecResult Exec(string Command, ArgumentList Args, OptRewrite[] Rewrite)
        {
            var A = GetAction(Command);
            if (A != null)
            {
                //var Cmd = ProcessCommand(A.Command, Args, Rewrite);

                string Dir = Args.Get("$dir");
                string P = (A.CustomToolPath != null) ? A.CustomToolPath : ToolPath;
                if (!File.Exists(P)) return new ERError($"No specified path for tool: {P}");
                ExternalToolResult Result;

                try
                {
                    if (Interpreter.Length > 0)
                    {
                        Result = ExternalExec.Exec(Interpreter, $"{P} {A.Command}", Args, Rewrite);
                    }
                    else
                    {
                        Result = ExternalExec.Exec(P, A.Command, Args, Rewrite);
                    }
                }
                catch (Exception Exc)
                {
                    return new ERError(Exc.Message);
                }
                //string Output = ExternalTool.Run(P, Cmd, Dir);
                if (Result == null) return new ERError("No output");
                
                var E = GetMessage(Result, A.ErrorMask);
                if (E != null) return new ERError(E);

                var W = GetMessage(Result, A.WarningMask);
                if (W != null) return new ERWarning(W);

                return new EROk();
            }
            else
                return new ERError($"No available command '{Command}'");
        }
    }
}
