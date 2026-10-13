using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Options.Rewrite
{
    /// <summary>
    /// Замена подстановкой из другого параметра
    /// </summary>
    public class ORAlias : OptRewrite
    {
        /// <summary>
        /// Из какого параметра берём значение
        /// </summary>
        protected string Alias;

        /// <summary>
        /// Получить значение
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public override string GetValue(string Name, ArgumentList Args)
        {
            return Args.Get(Alias);
        }

        public ORAlias(string Name, string Alias) : base(Name)
        {
            this.Alias = Alias;
        }
    }
}
