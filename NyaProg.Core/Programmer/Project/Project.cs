using Extension.Argument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Project
{
    public class Project
    {
        /// <summary>
        /// Название проекта
        /// </summary>
        public string Name;

        /// <summary>
        /// Описание проекта
        /// </summary>
        public string Description;

        /// <summary>
        /// Описание подробное, что к чему
        /// </summary>
        public string Readme;

        /// <summary>
        /// Документация: путь к файлу (относительно проекта) или URL.
        /// </summary>
        public string Doc;

        /// <summary>
        /// Папка, где лежит проект (устанавливается при загрузке)
        /// </summary>
        public string Dir;

        /// <summary>
        /// Файл проекта
        /// </summary>
        public string File;

        /// <summary>
        /// Дата создания (строка, как указано в описании проекта)
        /// </summary>
        public string Date;

        /// <summary>
        /// Изображение проекта (путь относительно каталога проекта)
        /// </summary>
        public string Photo;

        /// <summary>
        /// Список тегов
        /// </summary>
        public List<string> Tags = new List<string>();

        /// <summary>
        /// Поддерживаемые способы прошивки (программаторами, бутлоадеры, и т.д.)
        /// </summary>
        public List<string> Arch = new List<string>();

        /// <summary>
        /// Список алгоритмов прошивки
        /// </summary>
        public List<Target> Targets = new List<Target>();

        /// <summary>
        /// Опции
        /// </summary>
        public List<Options.Option> Options = new List<Options.Option>();

        public string GetID(Target Target)
        {
            if (Target != null)
            {
                var ID = Target.Arguments.Get("id");
                if (ID == null) ID = Target.Name;

                return $"project:{File}:{ID}";
            }
            else
                return "noscript";
        }

        public override string ToString()
        {
            return $"{Name}. {Description}";
        }

        public ArgumentList GetDefaultOptionsValues()
        {
            var A = new ArgumentList();
            foreach (var O in Options)
                A.Append(O.GetDefault());
            return A;
        }
    }
}
