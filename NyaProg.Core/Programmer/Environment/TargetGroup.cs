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
    public class TargetGroup
    {
        /// <summary>
        /// Название категории
        /// </summary>
        public string Name;

        /// <summary>
        /// Скрипт
        /// </summary>
        public List<Target> Targets = new List<Target>();

        public TargetGroup(string Name)
        {
            this.Name = Name;
        }
    }
}
