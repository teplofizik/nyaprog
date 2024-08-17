using Extension.Argument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Options
{
    public abstract class Option
    {
        /// <summary>
        /// Уникальное имя (для сохранения предпочтений)
        /// </summary>
        public string ID;

        /// <summary>
        /// Название опции
        /// </summary>
        public string Name;

        /// <summary>
        /// Может ли автоувеличиваться
        /// </summary>
        public bool Autoincrement = true;

        public Option(string N)
        {
            Name = N;
            ID = null;
        }
        
        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        /// <returns></returns>
        public virtual ArgumentList GetDefault() => new ArgumentList();

        public override string ToString()
        {
            return (Name != null) ? Name : base.ToString();
        }
    }
}
