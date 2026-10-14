using System;
using System.Collections.Generic;

using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>
    /// Модель экрана списка проектов: поиск + карточки.
    /// </summary>
    public sealed class ProjectListModel
    {
        private readonly Session Session;

        public ProjectListModel(Session Session) { this.Session = Session; }

        /// <summary>Строка поиска (по имени, описанию, тегам).</summary>
        public string Search { get; set; } = "";

        public IReadOnlyList<ProjectCard> Items()
        {
            var Res = new List<ProjectCard>();

            foreach (var P in Session.Env.Projects)
                if (Matches(P)) Res.Add(new ProjectCard(P));

            return Res;
        }

        private bool Matches(Project P)
        {
            var Q = (Search ?? "").Trim();
            if (Q.Length == 0) return true;

            if (Contains(P.Name, Q) || Contains(P.Description, Q) || ContainsTags(P, Q))
                return true;

            foreach (var A in P.Arch)
                if (Contains(A, Q)) return true;

            return false;
        }

        private static bool Contains(string Text, string Q) =>
            (Text != null) && (Text.IndexOf(Q, StringComparison.OrdinalIgnoreCase) >= 0);

        private static bool ContainsTags(Project P, string Q)
        {
            foreach (var T in P.Tags)
                if (Contains(T, Q)) return true;

            return false;
        }
    }
}
