using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment.Conditions.Expression
{
    class CondHasOption : CondExpression
    {
        /// <summary>
        /// Название опции
        /// </summary>
        private string Option;

        /// <summary>
        /// Проверять наличие опции (true) или отсутствие (false)
        /// </summary>
        private bool Avail;

        public CondHasOption(string Option, bool Avail)
        {
            this.Option = Option;
        }

        public override bool Check(State S)
        {
            bool Res = (S.CompiledArgs.Get(Option) != null);

            return (Avail) ? Res : !Res;
        }
    }
}
