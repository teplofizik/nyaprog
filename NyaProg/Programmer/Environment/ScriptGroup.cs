using Programmer.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment
{
    /// <summary>
    /// Группа, объединяющая несколько скриптов
    /// </summary>
    public class ScriptGroup
    {
        /// <summary>
        /// Название категории
        /// </summary>
        public string Name;

        /// <summary>
        /// Скрипт
        /// </summary>
        public List<Script> Scripts = new List<Script>();

        public ScriptGroup(string Name)
        {
            this.Name = Name;
        }
    }
}
