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
    public class Target
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
        /// Описание (кратко)
        /// </summary>
        public string Description;

        /// <summary>
        /// Описание подробное
        /// </summary>
        public string Readme;

        /// <summary>
        /// Документация: путь к файлу (относительно проекта) или URL.
        /// </summary>
        public string Doc;

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
        /// Список шагов (дерево: команды, set, if)
        /// </summary>
        public List<Step> Steps = new List<Step>();

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
        /// Построить план этапов: развернуть ветвления (if/else) по текущему
        /// окружению и отфильтровать шаги по их условиям.
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public List<Step> GetSteps(State S)
        {
            var Res = new List<Step>();
            Flatten(Steps, S, Res);
            return Res;
        }

        private static void Flatten(List<Step> In, State S, List<Step> Out)
        {
            foreach (var St in In)
            {
                if (St == null) continue;

                var If = St as IfStep;
                if (If != null)
                {
                    var Branch = ConditionChecker.Check(If.Condition, S) ? If.Then : If.Else;
                    Flatten(Branch, S, Out);
                }
                else
                    Out.Add(St);
            }
        }
    }
}
