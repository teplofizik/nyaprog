using System.Collections.Generic;

using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Карточка проекта для списка.
    /// </summary>
    public sealed class ProjectCard
    {
        public Project Project { get; }

        public ProjectCard(Project Project) { this.Project = Project; }

        public string Name => Project.Name;

        public string Description => Project.Description;

        public string Date => Project.Date;

        public string PhotoPath => PhotoResolver.Resolve(Project.Dir, Project.Photo);

        public IReadOnlyList<string> Tags => Project.Tags;

        /// <summary>Отметки типов прошиваемых контроллеров.</summary>
        public IReadOnlyList<string> ControllerTypes => Project.Arch;

        public int TargetCount => Project.Targets.Count;
    }
}
