using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Argument;
using Programmer.Modules;
using Programmer.Project;
using Programmer.Tool;
using System.IO;

namespace Programmer.Environment
{
    /// <summary>
    /// Выполнение скрипта
    /// </summary>
    public class TargetExec
    {
        /// <summary>
        /// Получить тип архитектуры, под какую надо искать программатор
        /// </summary>
        /// <param name="P"></param>
        /// <param name="S"></param>
        /// <returns></returns>
        public string[] GetTargetArch(Project.Project P, Target S)
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
        public ArgumentList GetArgs(Modules.ITool T, Project.Project P, Target S, Project.Action A, ArgumentList Options)
        {
            var Res = new ArgumentList();
            string Root = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location));
            string Soft = Path.Combine(Root, "Soft");

            // Этап 0
            // Пропишем параметры проекта (путь)
            Res.Set("$dir", P.Dir);
            Res.Set("dir", P.Dir);
            Res.Set("root", Root);
            Res.Set("soft", Soft);

            // Этап 1
            // Аргументы из скрипта (тип контроллера и т.д.)
            foreach (var K in S.Arguments.Keys)
                Res.Set(K, S.Arguments.Get(K));

            // Этап 2
            // Аргументы из программатора, если они есть (дефолтные)
            if (T != null)
            {
                foreach (var O in T.GetOptions())
                {

                }
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
        public ExecResult Exec(ProgEnv E, Modules.ITool T, Project.Project P, Target S, Project.Action A, ArgumentList Options)
        {
            var Command = A.Command;

            // Команда выбранного программатора имеет приоритет; иначе ищем по реестру.
            var Preferred = T;

            ICommand Cmd;
            if (E.Commands.TryResolve(Command, Preferred, out Cmd))
            {
                // Сформируем таблицу параметров
                var FullArgs = GetArgs(T, P, S, A, Options);

                // Применим схему параметров команды (defaults/обязательность/тип)
                string Error;
                if (!CommandBinder.Bind(Cmd, FullArgs, E.InputTypes, out Error))
                    return new ERError(Error);

                // Нативные команды: раскрыть {переменные} в значениях аргументов
                if (Cmd.ExpandArgs)
                    ExpandArgs(FullArgs, E);

                var Ctx = new CommandContext(Command, FullArgs, P.Dir, E);
                var R = Cmd.Execute(Ctx);

                // Выходные переменные команды — в окружение запуска (для условий/следующих шагов)
                if ((Options != null) && (R.Outputs != null))
                {
                    foreach (var K in R.Outputs.Keys)
                        Options.Set(K, R.Outputs.Get(K));
                }

                return Map(R);
            }
            else
                return new ERError($"Not available command: {Command}");
        }

        /// <summary>Раскрыть {переменные} в значениях аргументов (для нативных команд).</summary>
        private static void ExpandArgs(ArgumentList Args, ProgEnv E)
        {
            foreach (var K in Args.Keys)
            {
                var V = Args.Get(K);
                if (string.IsNullOrEmpty(V) || (V.IndexOf('{') < 0)) continue;

                try
                {
                    Args.Set(K, CommandTemplating.Process(V, N => E.ResolveArgument(N, Args) ?? ("{" + N + "}")));
                }
                catch { }
            }
        }

        /// <summary>
        /// Преобразовать результат команды к legacy-формату.
        /// </summary>
        private static ExecResult Map(CommandResult R)
        {
            switch (R.Status)
            {
                case CommandStatus.Ok: return new EROk();
                case CommandStatus.Warning: return new ERWarning(R.Message);
                default: return new ERError(R.Message);
            }
        }
    }
}
