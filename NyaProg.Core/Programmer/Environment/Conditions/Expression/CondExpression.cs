using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment.Conditions.Expression
{
    /// <summary>
    /// Проверка условий
    /// </summary>
    public class CondExpression
    {
        public virtual bool Check(State S) => true;
    }
}
