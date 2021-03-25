using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Project
{
    public class InputParam
    {
        /// <summary>
        /// Название параметра
        /// </summary>
        public string Name = "input";

        /// <summary>
        /// Тип параметра
        /// </summary>
        public string Type;

        /// <summary>
        /// Идентификатор для сохранения значений
        /// </summary>
        public string ID;

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment = "";

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public string Default = "";
    }
}
