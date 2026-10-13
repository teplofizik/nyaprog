using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Tool.XMLTools.XML
{
    public class XMLToolAction
    {
        // Команда программатору
        // {device} - название контроллера
        // {option} - опции
        // {filename} - имя файла
        public string Command = "";

        // Для отдельных действий
        public string Name = "";

        // Свой путь до ПО (для дополнительных программ)
        public string CustomToolPath = null;

        // Определение ошибки в выводе утилиты
        public List<XMLResultDetector> ErrorMask = new List<XMLResultDetector>();

        // Определение предупреждений в выводе утилиты
        public List<XMLResultDetector> WarningMask = new List<XMLResultDetector>();

        // Извлечение выходных переменных из вывода утилиты
        public List<XMLOutput> Outputs = new List<XMLOutput>();

        // Доступные опции
        public ArgumentList Defaults = new ArgumentList();
    }
}
