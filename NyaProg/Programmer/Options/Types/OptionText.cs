using Extension.Argument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Options.Types
{
    public class OptionText : Option
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
        /// Значение по умолчанию
        /// </summary>
        /// <returns></returns>
        public override ArgumentList GetDefault()
        {
            var A = new ArgumentList();
            A.Set(ParamName, Default);
            return A;
        }

        public OptionText(string Name) : base(Name)
        {

        }
    }
}
