using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Environment.Conditions.Expression;

namespace Programmer.Environment.Conditions
{
    public static class CondDetector
    {
        /// <summary>
        /// Определяет тип условия
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public static CondType GetCondition(string Cond)
        {
            if (Cond.Length == 0) return CondType.None;
            if (Cond.IndexOf("!=") > 0) return CondType.OptionNotEquals;
            if (Cond.IndexOf("==") > 0) return CondType.OptionEquals;
            if (Cond.IndexOf("!") == 0) return CondType.HasNotOption;

            return CondType.HasOption;
        }

        /// <summary>
        /// Получает объект, проверяющий условие
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public static CondExpression GetExpression(string Cond)
        {
            var CT = GetCondition(Cond);
            switch (CT)
            {
                case CondType.HasOption: return new CondHasOption(Cond, true);
                case CondType.HasNotOption: return new CondHasOption(Cond.Substring(1), false);
                case CondType.OptionEquals: return new CondEqual(GetOption(Cond, "=="), GetOptionValue(Cond, "=="));
                case CondType.OptionNotEquals: return new CondNotEqual(GetOption(Cond, "!="), GetOptionValue(Cond, "!="));
                default: return new CondExpression();
            }
        }

        /// <summary>
        /// Получает выражение перед оператором
        /// </summary>
        /// <param name="Cond"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        private static string GetOption(string Cond, string Operator)
        {
            var Ind = Cond.IndexOf(Operator);
            return Cond.Substring(0, Ind);
        }

        /// <summary>
        /// Получает выражение после оператора
        /// </summary>
        /// <param name="Cond"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        private static string GetOptionValue(string Cond, string Operator)
        {
            var Ind = Cond.IndexOf(Operator);
            return Cond.Substring(Ind + Operator.Length);
        }
    }
}
