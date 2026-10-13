using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Options.Rewrite
{
    public class OptHexExtractor : ORAlias
    {
        public OptHexExtractor(string Name, string Alias) : base(Name, Alias)
        {
        }

        private int? GetIndex(string Name)
        {
            if (String.Compare(this.Name, 0, Name, 0, this.Name.Length) == 0)
            {
                // Окей, начало прошло, проверим номер байта теперь
                string Arg = Name.Substring(this.Name.Length);

                int Val;
                if (Int32.TryParse(Arg, out Val))
                    return Val;
            }

            return null;
        }

        /// <summary>
        /// Проверить, подходит ли условие перезаписи под переданное название
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public override bool Check(string Name)
        {
            var I = GetIndex(Name);

            return I.HasValue;
        }
        /// <summary>
        /// Получить значение
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public override string GetValue(string Name, ArgumentList Args)
        {
            var V = base.GetValue(Name, Args);
            if (V != null)
            {
                int Id = GetIndex(Name).Value;
                if (IsHexOption(V))
                {
                    // Определим положение байта в строке
                    // MSB в начале строки
                    var Org = V.Length - 2 - Id * 2;
                    
                    if(Org >= 0)
                        return V.Substring(Org, 2);
                }
            }

            return null;
        }

        /// <summary>
        /// Это хекс?
        /// </summary>
        /// <param name="Option"></param>
        /// <returns></returns>
        private bool IsHexOption(string Option)
        {
            for (int i = 0; i < Option.Length; i++)
            {
                char C = Option[i];

                if (
                    ((C >= '0') && (C <= '9')) ||
                    ((C >= 'a') && (C <= 'f')) ||
                    ((C >= 'A') && (C <= 'F'))
                    ) continue;

                return false;
            }

            return true;
        }
    }
}
