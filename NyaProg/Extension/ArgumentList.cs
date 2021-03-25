using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.Argument
{
    public class ArgumentList
    {
        protected List<Argument> Args = new List<Argument>();

        protected Argument GetArg(string Name)
        {
            foreach (var A in Args)
            {
                if (A.Name.CompareTo(Name) == 0)
                    return A;
            }

            return null;
        }
        
        /// <summary>
        /// Добавить параметры
        /// </summary>
        /// <param name="Args"></param>
        public void Append(ArgumentList Args)
        {
            foreach (var K in Args.Keys)
                Set(K, Args.Get(K));
        }

        /// <summary>
        /// Получить значение аргумента или null
        /// </summary>
        /// <param name="Argument"></param>
        /// <returns></returns>
        public string Get(string Argument)
        {
            var A = GetArg(Argument);
            return (A != null) ? A.Value : null;
        }

        /// <summary>
        /// Задать значение аргумента
        /// </summary>
        /// <param name="Argument"></param>
        /// <param name="Value"></param>
        public void Set(string Argument, string Value)
        {
            var A = GetArg(Argument);

            if (A != null)
                A.Value = Value;
            else
                Args.Add(new Argument(Argument, Value));
        }

        public string[] Keys
        {
            get
            {
                var Res = new List<string>();
                foreach (var A in Args) Res.Add(A.Name);

                return Res.ToArray();
            }
        }

        public void Reset()
        {
            Args.Clear();
        }
    }
}
