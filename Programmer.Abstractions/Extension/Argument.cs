using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.Argument
{
    public class Argument
    {
        /// <summary>
        /// Название аргумента
        /// </summary>
        public string Name;

        /// <summary>
        /// Значение
        /// </summary>
        public string Value;

        public Argument(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
