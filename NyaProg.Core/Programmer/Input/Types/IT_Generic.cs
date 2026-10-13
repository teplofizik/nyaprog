using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Input.Types
{
    class IT_Generic : InputType
    {
        /// <summary>
        /// Проверка значения согласно типу
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected Func<string, bool> CheckFn;

        /// <summary>
        /// Преобразование значения в выходной формат
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected Func<string, string> ConvertFn;

        /// <summary>
        /// Инкремент
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected Func<string, string> IncrementFn;

        public IT_Generic(string Name, 
                          Func<string, bool> Chk, 
                          Func<string, string> Cnv, 
                          Func<string, string> Inc) : base(Name, Inc != null)
        {
            CheckFn = Chk;
            ConvertFn = Cnv;
            IncrementFn = Inc;
        }

        /// <summary>
        /// Проверка введённого значения
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override bool Check(string Value) => CheckFn(Value);

        /// <summary>
        /// Увеличение на 1
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override string Increment(string Value) => IncrementFn(Value);

        /// <summary>
        /// Преобразование в выходное значение
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public override string Convert(string Value) => ConvertFn(Value);
    }
}
