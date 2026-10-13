using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Options.Rewrite
{
    /// <summary>
    /// Алиасы к параметрам
    /// </summary>
    public class OptRewrite
    {
        /// <summary>
        /// Название параметра
        /// </summary>
        public string Name;

        /// <summary>
        /// Проверить, подходит ли условие перезаписи под переданное название
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public virtual bool Check(string Name) => (this.Name.CompareTo(Name) == 0);

        /// <summary>
        /// Получить значение
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public virtual string GetValue(string Name, ArgumentList Args) => null;

        public OptRewrite(string Name)
        {
            this.Name = Name;
        }
    }
}
