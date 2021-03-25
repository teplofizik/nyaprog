using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Environment;
using Programmer.Environment.Conditions;

namespace Programmer.Project
{
    /// <summary>
    /// Описывает набор команд для выполнения какого-то действия
    /// </summary>
    public class Script
    {
        /// <summary>
        /// Название скрипта
        /// </summary>
        public string Name;

        /// <summary>
        /// Категория
        /// </summary>
        public string Cathegory;

        /// <summary>
        /// Изображение
        /// </summary>
        public string Photo;

        /// <summary>
        /// Описание подробное
        /// </summary>
        public string Readme;

        /// <summary>
        /// Поддерживаемые способы прошивки (программаторами, бутлоадеры, и т.д. - если тип, указанный в проекте не подходит)
        /// </summary>
        public List<string> Arch = new List<string>();

        /// <summary>
        /// Аргументы (версия ПО, устаревшие варианты, тип контроллера, ...)
        /// </summary>
        public ArgumentList Arguments = new ArgumentList();

        /// <summary>
        /// Опции
        /// </summary>
        public List<Options.Option> Options = new List<Options.Option>();

        /// <summary>
        /// Список действий
        /// </summary>
        public List<Action> Actions = new List<Action>();

        /// <summary>
        /// Параметр для ввода пользователем
        /// </summary>
        public List<InputParam> Input = new List<InputParam>();

        public ArgumentList GetDefaultOptionsValues()
        {
            var A = new ArgumentList();
            foreach (var O in Options)
                A.Append(O.GetDefault());
            return A;
        }

        /// <summary>
        /// Получить этапы программирования в зависимости от программатора 
        /// (бутлоадер например, не может записать самого себя)
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public Action[] GetActions(State S)
        {
            var Acts = new List<Action>();

            var SE = new ScriptExec();
            foreach(var A in Actions)
            {
                if (A.Cond == null)
                    Acts.Add(A);
                else
                {
                    if(ConditionChecker.Check(A.Cond, S))
                        Acts.Add(A);
                }
            }

            return Acts.ToArray();
        }
    }
}
