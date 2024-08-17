using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment
{
    public class Config
    {
        /// <summary>
        /// Пути к файлам с поддерживаемыми программаторами
        /// </summary>
        public List<string> ProgToolsets = new List<string>();

        /// <summary>
        /// Пути к файлам с дополнительными командами
        /// </summary>
        public List<string> CustToolsets = new List<string>();

        /// <summary>
        /// Пути к проектам
        /// </summary>
        public List<string> Projects = new List<string>();

        /// <summary>
        /// Предпочтения
        /// </summary>
        public string Preferences = System.IO.Path.Combine("Base", "pref.xml");

        public Config()
        {
            ProgToolsets.Add("Tools");
            CustToolsets.Add(System.IO.Path.Combine("Tools", "Custom"));
            Projects.Add("Projects");
        }

        public void Load(string FileName)
        {
            //ProgToolsets.Clear();
            //CustToolsets.Clear();
        }
    }
}
