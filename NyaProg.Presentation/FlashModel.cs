using System;
using System.Collections.Generic;
using System.Linq;

using Extension.Argument;
using Programmer.Environment;
using Programmer.Options;
using Programmer.Options.Types;
using Programmer.Modules;
using Programmer.Project;
using Programmer.Tool;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Модель экрана прошивки конкретной цели: программатор, опции, план шагов, запуск.
    /// </summary>
    public sealed class FlashModel
    {
        private readonly Session Session;

        public Project Project { get; }

        public Target Target { get; }

        public IReadOnlyList<ITool> Programmers { get; private set; } = new ITool[0];

        public ITool Programmer { get; private set; }

        public IReadOnlyList<OptionModel> Options { get; private set; } = new OptionModel[0];

        public IReadOnlyList<StepItem> Steps { get; private set; } = new StepItem[0];

        public string Status { get; private set; } = "";

        public bool Running { get; private set; }

        public bool Cancelled { get; private set; }

        /// <summary>План/состояние изменились.</summary>
        public event System.Action Changed;

        /// <summary>Прогон завершён: true — успешно.</summary>
        public event System.Action<bool> Completed;

        public FlashModel(Session Session, Project Project, Target Target)
        {
            this.Session = Session;
            this.Project = Project;
            this.Target = Target;

            Session.State.ActionCompleted += OnActionCompleted;
            Build();
        }

        public string PhotoPath => PhotoResolver.Resolve(Project.Dir, Target.Photo);

        public string Readme => !string.IsNullOrEmpty(Target.Readme) ? Target.Readme : Project.Readme;

        /// <summary>Документация: файл (относительно проекта) или URL.</summary>
        public string Doc => !string.IsNullOrEmpty(Target.Doc) ? Target.Doc : Project.Doc;

        /// <summary>Собрать программаторы, опции и план.</summary>
        public void Build()
        {
            var State = Session.State;

            State.Project = Project;
            State.SelectTarget(Target);

            Programmers = Session.Env.GetToolsByArch(State.Arch);

            Programmer = FindProgrammer(State.Arch);

            State.Tool = Programmer;

            BuildOptions();
            ApplyOptions();
            RebuildPlan();
        }

        /// <summary>
        /// Выбор программатора: кэш проекта → дефолт по типу контроллера → первый доступный.
        /// </summary>
        private ITool FindProgrammer(string[] Arch)
        {
            var Pref = Session.Env.GetPreference(Project.GetID(Target));
            if (Pref != null)
            {
                var T = Programmers.FirstOrDefault(X => X.Name == Pref);
                if (T != null) return T;
            }

            foreach (var A in Arch)
            {
                var DA = Session.Env.GetPreference(Prefs.ArchProgrammer(A));
                if (DA != null)
                {
                    var T = Programmers.FirstOrDefault(X => X.Name == DA);
                    if (T != null) return T;
                }
            }

            return (Programmers.Count > 0) ? Programmers[0] : null;
        }

        public void SelectProgrammer(ITool Tool)
        {
            Programmer = Tool;
            Session.State.Tool = Tool;
            Session.Env.SetPreference(Project.GetID(Target), Tool?.Name ?? "");

            BuildOptions();
            ApplyOptions();
            RebuildPlan();
            Changed?.Invoke();
        }

        /// <summary>Пересчитать план шагов (после смены опций/программатора).</summary>
        public void RebuildPlan()
        {
            var State = Session.State;
            State.Plan = Target.GetSteps(State);

            var Items = new List<StepItem>();
            foreach (var S in State.Plan) Items.Add(new StepItem(S));
            Steps = Items;

            Changed?.Invoke();
        }

        public void ApplyOptions()
        {
            var State = Session.State;
            State.CompiledArgs = new ArgumentList();

            if (Programmer != null)
                foreach (var O in Programmer.GetOptions())
                    State.CompiledArgs.Append(O.GetDefault());

            State.CompiledArgs.Append(Project.GetDefaultOptionsValues());
            State.CompiledArgs.Append(Target.GetDefaultOptionsValues());

            foreach (var M in Options)
                State.CompiledArgs.Append(M.Args);
        }

        public void Start()
        {
            if (Running) return;

            ApplyOptions();
            SavePreferences();

            foreach (var S in Steps)
            {
                S.Running = false;
                S.Result = null;
                S.Ok = null;
            }

            Status = "";
            Running = true;
            Cancelled = false;
            Changed?.Invoke();

            StartStep(0);
        }

        public void Cancel()
        {
            Cancelled = true;
        }

        public void Detach()
        {
            Session.State.ActionCompleted -= OnActionCompleted;
        }

        /// <summary>Наполнить динамические опции значениями от команд-поставщиков.</summary>
        public void RefreshDynamic()
        {
            BuildOptions();
            ApplyOptions();
            RebuildPlan();
            Changed?.Invoke();
        }

        /// <summary>Обновить значения одной динамической опции у её поставщика.</summary>
        public void RefreshOption(OptionModel Model)
        {
            if ((Model == null) || (Model.Kind != OptionKind.Dynamic) || string.IsNullOrEmpty(Model.Source))
                return;

            Model.Items = Session.Env.GetListItems(Model.Source, Session.State.CompiledArgs ?? new ArgumentList());
            Changed?.Invoke();
        }

        private void BuildOptions()
        {
            var Raw = new List<Option>();
            if (Programmer != null) Raw.AddRange(Programmer.GetOptions());
            Raw.AddRange(Project.Options);
            Raw.AddRange(Target.Options);

            // Динамические списки — значения от поставщика (например, scan.ports)
            foreach (var O in Raw)
            {
                var OD = O as OptionDynamic;
                if (OD != null)
                    OD.Items = Session.Env.GetListItems(OD.Source, Session.State.CompiledArgs ?? new ArgumentList());
            }

            var Opts = new List<OptionModel>();
            foreach (var O in Raw) Opts.Add(OptionModel.FromOption(O));

            var IL = Session.Env.InputTypes;
            foreach (var I in Target.Input)
                Opts.Add(OptionModel.FromOption(new OptionInput(I.Name, I.Name, IL.GetInputType(I.Type), I.Default, I.ID, I.Autoincrement)));

            foreach (var M in Opts)
                if (M.ID != null) M.SetPreference(Session.Env.GetPreference(M.ID));

            Options = Opts;
        }

        private void StartStep(int Index)
        {
            if (Index >= Session.State.Plan.Count)
            {
                Finish(true);
                return;
            }

            if ((Index >= 0) && (Index < Steps.Count)) Steps[Index].Running = true;
            Session.State.RunAction(Index);
            Changed?.Invoke();
        }

        private void OnActionCompleted(object Sender, ActionEventArgs E)
        {
            var Item = ((E.Action >= 0) && (E.Action < Steps.Count)) ? Steps[E.Action] : null;
            if (Item != null)
            {
                Item.Running = false;
                Item.Result = E.Result.Message;
                Item.Ok = (E.Result.Result != ExecResultType.Error);
            }

            if (E.Result.Result == ExecResultType.Error)
            {
                Status = E.Result.Message;
                Finish(false);
                return;
            }

            if (Session.State.Action < Session.State.Plan.Count - 1)
            {
                if (Cancelled)
                {
                    Status = "Отменено";
                    Finish(false);
                }
                else
                    StartStep(Session.State.Action + 1);
            }
            else
            {
                Status = "Успешно выполнено!";
                Finish(true);
            }
        }

        private void Finish(bool Success)
        {
            Running = false;

            if (Success && Options.Any(M => M.Autoincrement))
                Autoincrement();

            Changed?.Invoke();
            Completed?.Invoke(Success);
        }

        private void Autoincrement()
        {
            foreach (var M in Options)
                if (M.Increment() && (M.ID != null))
                    Session.Env.SetPreference(M.ID, M.Preference);
        }

        private void SavePreferences()
        {
            foreach (var M in Options)
                if (M.ID != null) Session.Env.SetPreference(M.ID, M.Preference);
        }
    }
}
