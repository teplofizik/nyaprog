using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Tool;

namespace Programmer.Environment
{
    public class ActionEventArgs : EventArgs
    { 
        /// <summary>
        /// Номер действия
        /// </summary>
        public int Action;

        /// <summary>
        /// Результат действия
        /// </summary>
        public ExecResult Result;

        public ActionEventArgs(int Action, ExecResult Res)
        {
            this.Action = Action;
            Result = Res;
        }
    }

    public delegate void ActionEventHandler(object sender, ActionEventArgs e);
}
