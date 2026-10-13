using Programmer.Tool.XMLTools.XML;
using Programmer.XMLFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment
{
    public class Config
    {
        /// <summary>
        /// Путь к логу
        /// </summary>
        public string Log = "log.txt";

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
            AddDefault();
        }

        private static string PreprocessPath(string Path)
        {
            string Root = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location));
            string Soft = System.IO.Path.Combine(Root, "Soft");

            Path = Path.Replace("{root}", Root);
            Path = Path.Replace("{soft}", Soft);

            return Path;
        }

        private void AddDefault()
        {
            var Tools = System.IO.Path.Combine(PreprocessPath("{root}"), "Tools");
            if (ProgToolsets.Count == 0) ProgToolsets.Add(Tools);
            if (CustToolsets.Count == 0) CustToolsets.Add(System.IO.Path.Combine(Tools, "Custom"));
            if (Projects.Count == 0) Projects.Add(System.IO.Path.Combine(PreprocessPath("{root}"), "Projects"));
        }

        private void LoadDirs(XmlLoad X, List<string> Dirs)
        {
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "path": Dirs.Add(PreprocessPath(X.GetAttribute("value"))); break;
                }
            }

            X.Close();
        }

        public bool Load(string FileName)
        {
            XmlLoad X = new XmlLoad();
            if (!X.Load(FileName)) return false;

            ProgToolsets.Clear();
            CustToolsets.Clear();
            Projects.Clear();

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "log": Log = X.GetAttribute("value"); break;
                    case "toolkits": LoadDirs(X.GetSubtree(), ProgToolsets); break;
                    case "custom": LoadDirs(X.GetSubtree(), CustToolsets); break;
                    case "projects": LoadDirs(X.GetSubtree(), Projects); break;
                }
            }

            X.Close();

            // Проверяем...
            AddDefault();

            return true;
        }
    }
}
