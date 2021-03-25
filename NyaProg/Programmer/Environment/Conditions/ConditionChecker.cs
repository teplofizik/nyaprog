using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Environment.Conditions;

namespace Programmer.Environment.Conditions
{
    public static class ConditionChecker
    {
        public static bool Check(string Cond, State S)
        {
            var Exp = CondDetector.GetExpression(Cond);

            return Exp.Check(S);
        }
    }
}
