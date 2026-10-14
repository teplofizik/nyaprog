using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Карточка цели (контроллера) для экрана выбора.
    /// </summary>
    public sealed class TargetCard
    {
        public Project Project { get; }

        public Target Target { get; }

        public TargetCard(Project Project, Target Target)
        {
            this.Project = Project;
            this.Target = Target;
        }

        public string Name => Target.Name;

        public string Readme => Target.Readme;

        public string PhotoPath => PhotoResolver.Resolve(Project.Dir, Target.Photo);
    }
}
