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
    /// Сообщения
    /// </summary>
    class TlMessages : TlBase
    {
        public TlMessages() : base("Messages", new string[] { "message", "question" }) { }

        protected override ExecResult Exec(string Command, string Dir, string Option, ArgumentList Args, OptRewrite[] Rewrite)
        {
            switch(Command)
            {
                case "message": return Message(Dir, Option);
                case "question": return Question(Dir, Option);
                default: return base.Exec(Command, Dir, Option, Args, Rewrite);
            }
        }
        
        protected ExecResult Question(string Dir, string Option)
        {
            bool Result = MessageBox.Show(Option, "Вопрос", MessageBoxButtons.YesNo) == DialogResult.Yes;

            if(Result)
                return new EROk();
            else
                return new ERError($"Cancelled");
        }

        protected ExecResult Message(string Dir, string Option)
        {
            MessageBox.Show(Option, "Сообщение");
            return new EROk();
        }
        
    }
}
