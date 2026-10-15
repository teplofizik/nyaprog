using System;

using Programmer.Environment;
using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Контекст сеанса: окружение, состояние и текущая навигация
    /// (Список → Проект → Цель).
    /// </summary>
    public sealed class Session
    {
        public ProgEnv Env { get; }

        public State State { get; }

        public Project Project { get; private set; }

        public Target Target { get; private set; }

        public ProjectListModel Projects { get; }

        public TargetSelectModel Targets { get; private set; }

        public FlashModel Flash { get; private set; }

        public Session(ProgEnv Env)
        {
            this.Env = Env ?? throw new ArgumentNullException(nameof(Env));
            State = new State(Env);
            Projects = new ProjectListModel(this);
        }

        /// <summary>Открыть проект (экран выбора цели).</summary>
        public void Open(Project P)
        {
            Detach();

            Project = P;
            Target = null;

            State.SelectProject(P);
            Targets = new TargetSelectModel(P);
        }

        /// <summary>Открыть цель (экран прошивки).</summary>
        public void Open(Target T)
        {
            Detach();

            Target = T;

            State.Project = Project;
            State.SelectTarget(T);

            Flash = new FlashModel(this, Project, T);
        }

        /// <summary>Вернуться к списку проектов.</summary>
        public void Home()
        {
            Detach();

            Project = null;
            Target = null;

            State.SelectTarget(null);
        }

        private void Detach()
        {
            if (Flash != null)
            {
                Flash.Detach();
                Flash = null;
            }

            Targets = null;
        }
    }
}
