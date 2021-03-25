using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Project;
using Programmer.Tool;
using System.IO;

namespace Programmer.Environment
{
    /// <summary>
    /// Выполнение скрипта
    /// </summary>
    public class ScriptExec
    {
        /// <summary>
        /// Получить тип архитектуры, под какую надо искать программатор
        /// </summary>
        /// <param name="P"></param>
        /// <param name="S"></param>
        /// <returns></returns>
        public string[] GetScriptArch(Project.Project P, Script S)
        {
            if (S.Arch.Count > 0)
                return S.Arch.ToArray();
            else
                return P.Arch.ToArray();
        }

        /// <summary>
        /// Получить полный список аргументов к команде
        /// </summary>
        /// <param name="T">Программатор</param>
        /// <param name="P">Проект</param>
        /// <param name="S">Скрипт</param>
        /// <param name="Options">Опции программатора (из доступных к выбору пользователя) и пользовательский ввод</param>
        /// <returns></returns>
        public ArgumentList GetArgs(UniTool T, Project.Project P, Script S, Project.Action A, ArgumentList Options)
        {
            var Res = new ArgumentList();
            string Soft = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location)) + "\\Soft";

            // Этап 0
            // Пропишем параметры проекта (путь)
            Res.Set("$dir", P.Dir);
            Res.Set("dir", P.Dir);
            Res.Set("soft", Soft);

            // Этап 1
            // Аргументы из скрипта (тип контроллера и т.д.)
            foreach (var K in S.Arguments.Keys)
                Res.Set(K, S.Arguments.Get(K));

            // Этап 2
            // Аргументы из программатора, если они есть (дефолтные)
            foreach (var O in T.GetOptions())
            {

            }

            // Этап 3
            // Аргументы из команды (параметры команды)
            foreach (var K in A.Arguments.Keys)
                Res.Set(K, A.Arguments.Get(K));

            // Этап 4
            // Пропишем опции из ввода пользователя
            foreach (var K in Options.Keys)
                Res.Set(K, Options.Get(K));

            return Res;
        }

        /// <summary>
        /// Выполнить команду скрипта
        /// </summary>
        /// <param name="E">Окружение</param>
        /// <param name="T">Программатор</param>
        /// <param name="P">Проект</param>
        /// <param name="S">Скрипт</param>
        /// <param name="Options">Опции программатора (из доступных к выбору пользователя) и пользовательский ввод</param>
        /// <returns></returns>
        public ExecResult Exec(ProgEnv E, UniTool T, Project.Project P, Script S, Project.Action A, ArgumentList Options)
        {
            var Command = A.Command;
            UniTool ExecT = null;

            // Проверим, относится ли это к программатору
            if (T.CmdAvail(Command))
                ExecT = T;
            else
            {
                // Это не команда программатору, поищем в библиотеке утилит
                var Tool = E.GetToolByCommand(Command);
                if (Tool != null) ExecT = Tool;
            }

            if (ExecT != null)
            {
                // Есть такая команда в библиотеке
                // Сформируем таблицу параметров
                var FullArgs = GetArgs(T, P, S, A, Options);

                return ExecT.Exec(Command, FullArgs, E.OptRewriteRules);
            }
            else
                return new ERError($"Not available command: {Command}");
        }
    }
}
