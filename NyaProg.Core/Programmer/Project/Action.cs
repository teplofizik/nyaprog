using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Project
{
    /// <summary>
    /// Описывает одно действие
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Описание действия (для пользователя)
        /// </summary>
        public string Comment;

        /// <summary>
        /// Команда
        /// </summary>
        public string Command;

        // Условие доступности опции (если указано
        public string Cond;

        /// <summary>
        /// Аргументы
        /// </summary>
        public ArgumentList Arguments = new ArgumentList();

        public Action(string Command)
        {
            this.Command = Command;
        }
    }
}
