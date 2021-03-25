using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Options.Types
{
    public class OptionListItem
    {
        private ArgumentList Args = new ArgumentList();

        public string Label
        {
            protected set;
            get;
        }

        /// <summary>
        /// Элемент по умолчанию
        /// </summary>
        public bool Default = false;

        public OptionListItem(string L)
        {
            Label = L;
        }

        public override string ToString()
        {
            return (Label != null) ? Label : base.ToString();
        }
        
        /// <summary>
        /// Получить значение аргумента или null
        /// </summary>
        /// <param name="Argument"></param>
        /// <returns></returns>
        public string GetValue(string Argument)
        {
            return Args.Get(Argument);
        }

        /// <summary>
        /// Добавить значение в список
        /// </summary>
        /// <param name="Argument"></param>
        /// <param name="Value"></param>
        public void addValue(string Argument, string Value)
        {
            Args.Set(Argument, Value);
        }

        public string[] getNames() => Args.Keys;

        public ArgumentList Values
        {
            get
            {
                var AL = new ArgumentList();
                AL.Append(Args);
                return AL;
            }
        }
    }
}
