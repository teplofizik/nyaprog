using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Extension.Argument;
using Programmer.Options.Rewrite;

namespace Programmer.Tool.InternalTools
{
    /// <summary>
    /// Вызов внешних программ (команда external)
    /// </summary>
    class TlExternal : TlBase
    {
        public TlExternal() : base("External", new string[] { "external" }) { }

        protected override ExecResult Exec(string Command, string Dir, string Option, ArgumentList Args, OptRewrite[] Rewrite)
        {
            switch(Command)
            {
                case "external": return External(Dir, Option, Args, Rewrite);
                default: return base.Exec(Command, Dir, Option, Args, Rewrite);
            }
        }
        
        protected ExecResult External(string Dir, string Option, ArgumentList Args, OptRewrite[] Rewrite)
        {
             int Separator = Option.IndexOf(".exe");
             if (Separator == -1) return new ERError($"Fail to detect .exe");

            string Exe = Option.Substring(0, Separator + 4);
            string ExeArgs = Option.Substring(Separator + 5);

            ExternalToolResult Output = ExternalExec.Exec(Exe, ExeArgs, Args, Rewrite);

            if (Output != null)
            {
                if(Output.ExitCode == 0)
                    return new EROk();
                else
                    return new ERError(Output.Result);
            }
            else
                return new ERError($"Fail");
        }
    }
}
