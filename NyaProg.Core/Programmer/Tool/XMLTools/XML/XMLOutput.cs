using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Tool.XMLTools.XML
{
    /// <summary>
    /// Правило извлечения значения из вывода утилиты в переменную команды.
    /// </summary>
    public class XMLOutput
    {
        /// <summary>Имя выходной переменной.</summary>
        public string Name = "";

        /// <summary>Регулярное выражение.</summary>
        public string Pattern = "";

        /// <summary>Номер группы (по умолчанию 1).</summary>
        public int Group = 1;
    }
}
