using System.Collections.Generic;
using System.Linq;

using Programmer.Project;

namespace NyaProg.Presentation
{
    /// <summary>Редактируемый шаг-команда.</summary>
    public sealed class StepEdit
    {
        public string Command = "wflash";
        public string Value = "";
        public string Comment = "";
    }

    /// <summary>Редактируемая цель (контроллер).</summary>
    public sealed class TargetEdit
    {
        public string Name = "";
        public string Device = "";
        public string Type = "";
        public string Photo = "";
        public string Doc = "";
        public string Description = "";
        public string Readme = "";
        public List<StepEdit> Steps = new List<StepEdit>();

        /// <summary>Исходная цель (null — новая): нередактируемые поля сохраняются.</summary>
        public Target Source;

        /// <summary>Есть шаги set/if — они не редактируются и сохраняются как есть.</summary>
        public bool HasComplexSteps;
    }

    /// <summary>
    /// UI-независимая модель редактора проекта (создание/простое редактирование).
    /// Условия (set/if) не редактируются.
    /// </summary>
    public sealed class ProjectEditorModel
    {
        public string Name = "";
        public string Description = "";
        public string Readme = "";
        public string Date = "";
        public string Type = "";
        public string Photo = "";
        public string Doc = "";
        public string File;
        public string Dir;
        public List<TargetEdit> Targets = new List<TargetEdit>();

        /// <summary>Исходный проект (null — новый): нередактируемые поля (теги, опции, ...) сохраняются.</summary>
        public Project Source;

        public bool IsNew => string.IsNullOrEmpty(File);

        public static ProjectEditorModel FromProject(Project P)
        {
            var M = new ProjectEditorModel
            {
                Name = P.Name ?? "",
                Description = P.Description ?? "",
                Readme = P.Readme ?? "",
                Date = P.Date ?? "",
                Type = (P.Arch.Count > 0) ? P.Arch[0] : "",
                Photo = P.Photo ?? "",
                Doc = P.Doc ?? "",
                File = P.File,
                Dir = P.Dir,
                Source = P
            };

            foreach (var T in P.Targets)
            {
                var TE = new TargetEdit
                {
                    Name = T.Name ?? "",
                    Device = T.Arguments.Get("device") ?? "",
                    Type = (T.Arch.Count > 0) ? T.Arch[0] : "",
                    Photo = T.Photo ?? "",
                    Doc = T.Doc ?? "",
                    Description = T.Description ?? "",
                    Readme = T.Readme ?? "",
                    Source = T,
                    HasComplexSteps = T.Steps.Any(S => !(S is Action))
                };

                foreach (var S in T.Steps)
                {
                    var Cmd = S as Action;
                    if (Cmd == null) continue;

                    TE.Steps.Add(new StepEdit
                    {
                        Command = Cmd.Command ?? "",
                        Value = Cmd.Arguments.Get("value") ?? "",
                        Comment = Cmd.Comment ?? ""
                    });
                }

                M.Targets.Add(TE);
            }

            if (M.Targets.Count == 0) M.Targets.Add(new TargetEdit());

            return M;
        }

        public Project ToProject()
        {
            // Существующий проект правим на месте: теги, опции, input, доп. аргументы
            // и шаги set/if сохраняются (редактор их не показывает).
            var P = Source ?? new Project();

            P.Name = Name;
            P.Description = NullIfEmpty(Description);
            P.Readme = NullIfEmpty(Readme);
            P.Date = Date;
            P.Photo = NullIfEmpty(Photo);
            P.Doc = NullIfEmpty(Doc);
            P.File = File;
            P.Dir = Dir;

            if (!string.IsNullOrEmpty(Type))
            {
                if (P.Arch.Count == 0) P.Arch.Add(Type);
                else P.Arch[0] = Type;
            }

            P.Targets.Clear();
            foreach (var TE in Targets)
            {
                var T = TE.Source ?? new Target();

                T.Name = TE.Name;
                T.Photo = NullIfEmpty(TE.Photo);
                T.Doc = NullIfEmpty(TE.Doc);
                T.Description = NullIfEmpty(TE.Description);
                T.Readme = NullIfEmpty(TE.Readme);

                if (!string.IsNullOrEmpty(TE.Type))
                {
                    if (T.Arch.Count == 0) T.Arch.Add(TE.Type);
                    else T.Arch[0] = TE.Type;
                }

                if (!string.IsNullOrEmpty(TE.Device)) T.Arguments.Set("device", TE.Device);

                // Шаги перестраиваем только если нет set/if — иначе сохраняем исходные.
                if (!TE.HasComplexSteps)
                {
                    T.Steps.Clear();
                    foreach (var SE in TE.Steps)
                    {
                        if (string.IsNullOrEmpty(SE.Command)) continue;

                        var A = new Action(SE.Command) { Comment = NullIfEmpty(SE.Comment) };
                        if (!string.IsNullOrEmpty(SE.Value)) A.Arguments.Set("value", SE.Value);

                        T.Steps.Add(A);
                    }
                }

                P.Targets.Add(T);
            }

            return P;
        }

        private static string NullIfEmpty(string S) => string.IsNullOrEmpty(S) ? null : S;
    }
}
