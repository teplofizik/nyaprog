using System;

using Extension.Argument;
using Programmer.Environment.Conditions.Expressions;

namespace Programmer.Environment.Conditions
{
    /// <summary>
    /// Проверка условий шага. Поддерживает выражения: переменные, литералы,
    /// <c>! ( ) &amp;&amp; ||</c> и сравнения <c>== != &lt; &gt; &lt;= &gt;=</c>.
    /// Пустое условие считается истинным.
    /// </summary>
    public static class ConditionChecker
    {
        public static bool Check(string Cond, State S)
        {
            if (string.IsNullOrWhiteSpace(Cond)) return true;

            try
            {
                var Args = (S != null) ? S.CompiledArgs : null;
                return ExpressionParser.Parse(Cond).Test(Args ?? new ArgumentList());
            }
            catch (Exception)
            {
                // Некорректное условие не должно ломать построение плана.
                return false;
            }
        }
    }
}
