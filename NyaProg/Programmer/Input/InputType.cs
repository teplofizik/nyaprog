using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Input
{
    public abstract class InputType
    {
        /// <summary>
        /// Тип ввода
        /// </summary>
        public string Type;

        /// <summary>
        /// Может увеличиваться
        /// </summary>
        public readonly bool CanIncrement;

        /// <summary>
        /// Проверка введённого значения
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public abstract bool Check(string Value);

        /// <summary>
        /// Преобразование в выходное значение
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public abstract string Convert(string Value);

        /// <summary>
        /// Увеличение на 1
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public abstract string Increment(string Value);

        public InputType(string TypeName, bool Incrementable)
        {
            Type = TypeName;
            CanIncrement = Incrementable;
        }
    }
}
