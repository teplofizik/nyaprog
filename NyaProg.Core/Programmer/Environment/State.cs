using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Programmer.Project;
using Programmer.Tool;
using Programmer.Platform;
using System.Threading;
using Extension.Argument;

namespace Programmer.Environment
{
    /// <summary>
    /// Текущее состояние ПО (выбранный проект, параметры программирования, 
    /// запомненные значения полей ввода и т.д.)
    /// </summary>
    public class State
    {
        /// <summary>
        /// Шаг завершён
        /// </summary>
        public event ActionEventHandler ActionCompleted;

        /// <summary>
        /// Окружение
        /// </summary>
        public ProgEnv Env;

        /// <summary>
        /// Выбранные проект (если есть)
        /// </summary>
        public Project.Project Project;

        /// <summary>
        /// Скрипты, раз
        /// </summary>
        public List<TargetGroup> Groups = new List<TargetGroup>();

        /// <summary>
        /// Выбранный скрипт
        /// </summary>
        public Target Target;

        /// <summary>
        /// План выполнения: шаги с развёрнутыми ветвлениями и применёнными условиями.
        /// </summary>
        public List<Step> Plan = new List<Step>();

        /// <summary>
        /// Выбранный выполнятор скрипта
        /// </summary>
        public TargetExec Executer;

        /// <summary>
        /// Выбранный программатор
        /// </summary>
        public UniTool Tool;
        
        /// <summary>
        /// Выбранные архитектуры (для выбора программаторов)
        /// </summary>
        public string[] Arch => (Executer != null) ? Executer.GetTargetArch(Project, Target) : new string[] { };

        /// <summary>
        /// Номер шага
        /// </summary>
        public int Action = 0;

        /// <summary>
        /// Отменено
        /// </summary>
        public bool Cancelled = false;

        /// <summary>
        /// Аргументы от всех выбранных пользователем опций
        /// </summary>
        public ArgumentList CompiledArgs;

        /// <summary>
        /// Выбрать проект
        /// </summary>
        /// <param name="P"></param>
        public void SelectProject(Project.Project P)
        {
            Project = P;
            Groups = new List<TargetGroup>();

            foreach (var S in P.Targets)
            {
                var SG = GetTargetGroup(S.Cathegory);
                SG.Targets.Add(S);
            }
        }

        /// <summary>
        /// Выбрать проект
        /// </summary>
        /// <param name="P"></param>
        public void SelectTarget(Target S)
        {
            Action = 0;
            Target = S;
            Plan = new List<Step>();
            if (S != null)
            {
                Executer = new TargetExec();
            }
            else
                Executer = null;
        }

        /// <summary>
        /// Получить группу скриптов по названию категории
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private TargetGroup GetTargetGroup(string Name)
        {
            foreach (var G in Groups)
            {
                if (G.Name.CompareTo(Name) == 0)
                    return G;
            }

            var NG = new TargetGroup(Name);
            Groups.Add(NG);
            return NG;
        }

        /// <summary>
        /// Конструктор с указанием файла конфигурации
        /// </summary>
        /// <param name="Config"></param>
        public State(string Config, IUserInterface UI = null)
        {
            Env = new ProgEnv(UI);
            if (Config != null)
                Env.Load(Config);
        }
        
        /// <summary>
        /// Прошивка выполняется в параллельном потоке
        /// </summary>
        public void ActionThread()
        {
            var St = (Action < Plan.Count) ? Plan[Action] : null;

            ExecResult Result;

            var Set = St as SetStep;
            if (Set != null)
            {
                // Присвоение переменной окружения (значение — шаблон).
                if (CompiledArgs == null) CompiledArgs = new ArgumentList();

                try
                {
                    var V = Modules.CommandTemplating.Process(Set.Value ?? "", N => Env.ResolveArgument(N, CompiledArgs));
                    CompiledArgs.Set(Set.Name, V);
                    Result = new EROk();
                }
                catch (Exception E)
                {
                    Result = new ERError(E.Message);
                }
            }
            else if (St is Project.Action)
                Result = Executer.Exec(Env, Tool, Project, Target, (Project.Action)St, CompiledArgs);
            else
                Result = new ERError("Invalid step");

            Debug.WriteLine(String.Format("   {0:s}: {1:s}", (Result.Result != ExecResultType.Error) ? "OK" : "FAIL", Result.Message));
            ActionCompleted?.Invoke(this, new ActionEventArgs(Action, Result));
        }
        
        public void ResetArguments()
        {
            CompiledArgs = new ArgumentList();

            if (Tool != null) CompiledArgs.Append(Tool.GetDefaultOptionsValues());
            if (Project != null) CompiledArgs.Append(Project.GetDefaultOptionsValues());
            if (Target != null) CompiledArgs.Append(Target.GetDefaultOptionsValues());
        }

        public void RunAction(int Index)
        {
            Action = Index;
            Cancelled = false;
            if (Index < Plan.Count)
            {
                Thread Thr = new Thread(ActionThread);
                Thr.Start();
            }
            else
                ActionCompleted?.Invoke(this, new ActionEventArgs(Action, new ERError("Invalid action index")));
        }
    }
}
