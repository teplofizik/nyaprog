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
        /// <summary>
        /// Название параметра
        /// </summary>
        public string ParamName = "";

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public string Default = "";

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

        public OptionInput(string Name, string ParamName, InputType Type, string Default, string ID) : base(Name)
        {
            this.ParamName = ParamName;
            this.Default = Default;
            this.Type = Type;
            this.ID = ID;
        }
    }
}
