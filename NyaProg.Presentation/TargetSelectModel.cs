using System.Collections.Generic;

using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Модель экрана выбора цели (контроллера).
    /// </summary>
    public sealed class TargetSelectModel
    {
        public Project Project { get; }

        public TargetSelectModel(Project Project) { this.Project = Project; }

        public IReadOnlyList<TargetCard> Items()
        {
            var Res = new List<TargetCard>();

            foreach (var T in Project.Targets)
                Res.Add(new TargetCard(Project, T));

            return Res;
        }

        /// <summary>Экран можно пропустить, если цель одна.</summary>
        public bool Skip => Project.Targets.Count <= 1;
    }
}
