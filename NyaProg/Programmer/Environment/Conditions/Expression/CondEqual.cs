using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment.Conditions.Expression
{
    class CondEqual : CondExpression
    {
        /// <summary>
        /// Название опции
        /// </summary>
        private string Option;

        /// <summary>
        /// Значение опции
        /// </summary>
        private string Value;

        public CondEqual(string Option, string Value)
        {
            this.Option = Option;
            this.Value = Value;
        }

        public override bool Check(State S)
        {
            var OptVal = S.CompiledArgs.Get(Option);

            return (OptVal != null) && (OptVal.CompareTo(Value) == 0);
        }
    }
}
