using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Options.Rewrite;
using Programmer.Platform;

namespace Programmer.Tool.InternalTools
{
    /// <summary>
    /// Сообщения
    /// </summary>
    class TlMessages : TlBase
    {
        private readonly IUserInterface UI;

        public TlMessages(IUserInterface UI) : base("Messages", new string[] { "message", "question" })
        {
            this.UI = UI ?? new NullUserInterface();
        }

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
            bool Result = UI.Question(Option, "Вопрос");

            if(Result)
                return new EROk();
            else
                return new ERError($"Cancelled");
        }

        protected ExecResult Message(string Dir, string Option)
        {
            UI.Message(Option, "Сообщение");
            return new EROk();
        }
        
    }
}
