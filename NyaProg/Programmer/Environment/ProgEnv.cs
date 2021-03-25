using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Programmer.Tool;
using Programmer.Tool.Toolset;
using Programmer.Project;
using Programmer.Options.Rewrite;
using System.IO;
using Programmer.Input;

namespace Programmer.Environment
{
    public class ProgEnv
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        Config Conf = new Config();

        /// <summary>
        /// Список программаторов
        /// </summary>
        ToolSet ProgTS = new ProgToolset();

        /// <summary>
        /// Список утилит
        /// </summary>
        ToolSet CustTS = new CustomToolset();

        /// <summary>
        /// Список проектов
        /// </summary>
        ProjLibrary Lib = new ProjLibrary();

        /// <summary>
        /// Предпочтения в программаторах
        /// </summary>
        Preference Pref = new Preference();

        /// <summary>
        /// Библиотека типов пользовательского ввода
        /// </summary>
        InputTypeLibrary InTypes = new InputTypeLibrary();

        /// <summary>
        /// Перезапись параметров
        /// </summary>
        List<OptRewrite> OptionAlias = new List<OptRewrite>();

        /// **************************************************************
        /// Свойства
        /// **************************************************************

        /// <summary>
        /// Полный список загруженных проектов
        /// </summary>
        public Project.Project[] Projects => Lib.Projects.ToArray();

        /// <summary>
        /// Полный список загруженных программаторов
        /// </summary>
        public UniTool[] Tools => ProgTS.Tools.ToArray();
        
        /// <summary>
        /// Список правил перезаписи параметров
        /// </summary>
        public OptRewrite[] OptRewriteRules => OptionAlias.ToArray();

        /// <summary>
        /// Библиотека поддерживаемых типов пользовательского ввода
        /// </summary>
        public InputTypeLibrary InputTypes => InTypes;

        /// **************************************************************
        /// Методы
        /// **************************************************************

        /// <summary>
        /// Загрузить конфигурацию
        /// </summary>
        /// <param name="FileName"></param>
        protected void LoadConfig(string FileName) => Conf.Load(FileName);

        /// <summary>
        /// Выгрузить набор программаторов и утилит
        /// </summary>
        protected void UnloadToolset()
        {

        }

        /// <summary>
        /// Загрузить набор программаторов и утилит
        /// </summary>
        protected void LoadToolset()
        {
            UnloadToolset();

            ProgTS = new ProgToolset();
            CustTS = new CustomToolset();
            
            foreach(var P in Conf.ProgToolsets)  ProgTS.Load(P);
            foreach (var P in Conf.CustToolsets) CustTS.Load(P);
        }

        /// <summary>
        /// Выгрузить набор программаторов и утилит
        /// </summary>
        protected void UnloadProjects()
        {

        }

        /// <summary>
        /// Загрузить список проектов
        /// </summary>
        protected void LoadProjects()
        {
            UnloadProjects();

            Lib = new ProjLibrary();
            foreach (var P in Conf.Projects) Lib.Load(P);
        }

        /// <summary>
        /// Добавить перезапись параметров
        /// </summary>
        protected void LoadOptRewrite()
        {
            OptionAlias = new List<OptRewrite>();
            // Обычно в командах конфигурации используется option вместо value
            OptionAlias.Add(new ORAlias("option", "value"));
            // Обычно в командах записи ПО используется filename вместо value
            OptionAlias.Add(new ORAlias("filename", "value"));
            // То же, но без расширения
            OptionAlias.Add(new ORAliasConv("filenamewe", "value", s => Path.GetFileNameWithoutExtension(s)));
            // Переписывание опции побайтово (для фьюзов, напимер)
            OptionAlias.Add(new OptHexExtractor("option", "value"));
        }
        
        /// <summary>
        /// Полная загрузка окружения
        /// </summary>
        public void Load(string ConfigFN)
        {
            Conf = new Config();
            if(ConfigFN != null) LoadConfig(ConfigFN);

            LoadToolset();
            LoadProjects();
            LoadOptRewrite();
            Pref.Load(Conf.Preferences);
        }

        /// <summary>
        /// Сохранение окружения
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {
            Pref.Save(Conf.Preferences);
        }

        /// <summary>
        /// Список программаторов по списку поддерживаемых архитектур
        /// </summary>
        /// <param name="Arch"></param>
        /// <returns></returns>
        public UniTool[] GetToolsByArch(string[] Arch)
        {
            var Tools = new List<UniTool>();

            foreach(var T in ProgTS.Tools)
            {
                if (T.IsSupportedArch(Arch))
                    Tools.Add(T);
            }

            return Tools.ToArray();
        }

        /// <summary>
        /// Получить набор по названию команды
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public UniTool GetToolByCommand(string Command)
        {
            foreach (var T in CustTS.Tools)
            {
                if (T.CmdAvail(Command))
                    return T;
            }

            return null;
        }

        /// <summary>
        /// Установить предпочтения
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Value"></param>
        public void SetPreference(string ID, string Value)
        {
            Pref.Set(ID, Value);
        }

        /// <summary>
        /// Прочитать предпочтения
        /// </summary>
        /// <param name="ID"></param>
        public string GetPreference(string ID)
        {
            return Pref.Get(ID);
        }
    }
}
