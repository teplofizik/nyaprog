using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Project
{
    /// <summary>
    /// Шаг-команда: выполнить команду с аргументами.
    /// </summary>
    public class Action : Step
    {
        /// <summary>
        /// Команда
        /// </summary>
        public string Command;

        /// <summary>
        /// Аргументы
        /// </summary>
        public ArgumentList Arguments = new ArgumentList();

        public Action(string Command)
        {
            this.Command = Command;
        }

        public override string Title => Comment ?? Command;
    }
}
