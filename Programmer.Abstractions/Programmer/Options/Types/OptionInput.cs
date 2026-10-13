using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Input;

namespace Programmer.Options.Types
{
    /// <summary>
    /// Представляет собой пользовательский ввод
    /// </summary>
    public class OptionInput : Option
    {
        /// <summary
        /// <summary>
        /// Название параметра
        /// </summary>
        public readonly string ParamName;

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public readonly string Default;

        /// <summary>
        /// Тип вводного параметра
        /// </summary>
        public InputType Type = null;

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        /// <returns></returns>
        public override ArgumentList GetDefault()
        {
            var A = new ArgumentList();
            A.Set(ParamName, Default);
            return A;
        }

        public OptionInput(string Name, string ParamName, InputType Type, string Default, string ID, bool Autoincrement) : base(Name)
        {
            this.ParamName = ParamName;
            this.Default = Default;
            this.Type = Type;
            this.ID = ID;
            this.Autoincrement = Autoincrement;
        }
    }
}
