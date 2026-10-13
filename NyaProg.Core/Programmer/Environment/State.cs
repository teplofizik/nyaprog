using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Programmer.Project;
using Programmer.Tool;
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
        public List<ScriptGroup> Groups = new List<ScriptGroup>();

        /// <summary>
        /// Выбранный скрипт
        /// </summary>
        public Script Script;

        /// <summary>
        /// Выбранный выполнятор скрипта
        /// </summary>
        public ScriptExec Executer;

        /// <summary>
        /// Выбранный программатор
        /// </summary>
        public UniTool Tool;
        
        /// <summary>
        /// Выбранные архитектуры (для выбора программаторов)
        /// </summary>
        public string[] Arch => (Executer != null) ? Executer.GetScriptArch(Project, Script) : new string[] { };

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
            Groups = new List<ScriptGroup>();

            foreach (var S in P.Scripts)
            {
                var SG = GetScriptGroup(S.Cathegory);
                SG.Scripts.Add(S);
            }
        }

        /// <summary>
        /// Выбрать проект
        /// </summary>
        /// <param name="P"></param>
        public void SelectScript(Script S)
        {
            Action = 0;
            Script = S;
            if (S != null)
            {
                Executer = new ScriptExec();
            }
            else
                Executer = null;
        }

        /// <summary>
        /// Получить группу скриптов по названию категории
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private ScriptGroup GetScriptGroup(string Name)
        {
            foreach (var G in Groups)
            {
                if (G.Name.CompareTo(Name) == 0)
                    return G;
            }

            var NG = new ScriptGroup(Name);
            Groups.Add(NG);
            return NG;
        }

        /// <summary>
        /// Конструктор с указанием файла конфигурации
        /// </summary>
        /// <param name="Config"></param>
        public State(string Config)
        {
            Env = new ProgEnv();
            if (Config != null)
                Env.Load(Config);
        }
        
        /// <summary>
        /// Прошивка выполняется в параллельном потоке
        /// </summary>
        public void ActionThread()
        {
            var Result = Executer.Exec(Env, Tool, Project, Script, Script.Actions[Action], CompiledArgs);

            Debug.WriteLine(String.Format("   {0:s}: {1:s}", (Result.Result != ExecResultType.Error) ? "OK" : "FAIL", Result.Message));
            ActionCompleted?.Invoke(this, new ActionEventArgs(Action, Result));
        }
        
        public void ResetArguments()
        {
            CompiledArgs = new ArgumentList();

            if (Tool != null) CompiledArgs.Append(Tool.GetDefaultOptionsValues());
            if (Project != null) CompiledArgs.Append(Project.GetDefaultOptionsValues());
            if (Script != null) CompiledArgs.Append(Script.GetDefaultOptionsValues());
        }

        public void RunAction(int Index)
        {
            Action = Index;
            Cancelled = false;
            if (Index < Script.Actions.Count)
            {
                Thread Thr = new Thread(ActionThread);
                Thr.Start();
            }
            else
                ActionCompleted?.Invoke(this, new ActionEventArgs(Action, new ERError("Invalid action index")));
        }
    }
}
