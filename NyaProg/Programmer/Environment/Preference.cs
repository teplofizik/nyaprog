using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;

namespace Programmer.Environment
{
    class Preference
    {
        ArgumentList Pref = new ArgumentList();

        /// <summary>
        /// Установить предпочтения
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Value"></param>
        public void Set(string ID, string Value)
        {
            Pref.Set(ID, Value);
        }

        /// <summary>
        /// Прочитать предпочтения
        /// </summary>
        /// <param name="ID"></param>
        public string Get(string ID)
        {
            return Pref.Get(ID);
        }

        /// <summary>
        /// Загрузить
        /// </summary>
        /// <param name="FileName"></param>
        public void Load(string FileName)
        {
            Pref.Reset();
            XML.XMLLoader.Load(this, FileName);
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {
            XML.XMLWriter.Save(this, FileName);
        }

        /// <summary>
        /// Список ID
        /// </summary>
        public string[] ID => Pref.Keys;
    }
}
