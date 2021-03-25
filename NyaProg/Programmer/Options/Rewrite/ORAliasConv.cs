using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Options.Rewrite
{
    /// <summary>
    /// Замена подстановкой из другого параметра с преобразованием
    /// </summary>
    public class ORAliasConv : ORAlias
    {
        /// <summary>
        /// Функция преобразования
        /// </summary>
        private Func<string, string> Conv;

        /// <summary>
        /// Получить значение
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public override string GetValue(string Name, ArgumentList Args)
        {
            var V = base.GetValue(Name, Args);
            if(V != null)
                return (Conv != null) ? Conv(V) : V;
            else
                return null;
        }

        public ORAliasConv(string Name, string Alias, Func<string,string> Conv) : base(Name, Alias)
        {
            this.Conv = Conv;
        }
    }
}
