using System.Collections.Generic;

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
        public string Photo = "";
        public List<StepEdit> Steps = new List<StepEdit>();
    }

    /// <summary>
    /// UI-независимая модель редактора проекта (создание/простое редактирование).
    /// Условия (set/if) не редактируются.
    /// </summary>
    public sealed class ProjectEditorModel
    {
        public string Name = "";
        public string Description = "";
        public string Date = "";
        public string Type = "";
        public string Photo = "";
        public string File;
        public string Dir;
        public List<TargetEdit> Targets = new List<TargetEdit>();

        public bool IsNew => string.IsNullOrEmpty(File);

        public static ProjectEditorModel FromProject(Project P)
        {
            var M = new ProjectEditorModel
            {
                Name = P.Name ?? "",
                Description = P.Description ?? "",
                Date = P.Date ?? "",
                Type = (P.Arch.Count > 0) ? P.Arch[0] : "",
                Photo = P.Photo ?? "",
                File = P.File,
                Dir = P.Dir
            };

            foreach (var T in P.Targets)
            {
                var TE = new TargetEdit
                {
                    Name = T.Name ?? "",
                    Device = T.Arguments.Get("device") ?? "",
                    Photo = T.Photo ?? ""
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
            var P = new Project
            {
                Name = Name,
                Description = Description,
                Date = Date,
                Photo = (Photo.Length > 0) ? Photo : null,
                File = File,
                Dir = Dir
            };

            if (!string.IsNullOrEmpty(Type)) P.Arch.Add(Type);

            foreach (var TE in Targets)
            {
                var T = new Target { Name = TE.Name, Photo = (TE.Photo.Length > 0) ? TE.Photo : null };

                if (!string.IsNullOrEmpty(TE.Device)) T.Arguments.Set("device", TE.Device);

                foreach (var SE in TE.Steps)
                {
                    if (string.IsNullOrEmpty(SE.Command)) continue;

                    var A = new Action(SE.Command) { Comment = (SE.Comment.Length > 0) ? SE.Comment : null };
                    if (!string.IsNullOrEmpty(SE.Value)) A.Arguments.Set("value", SE.Value);

                    T.Steps.Add(A);
                }

                P.Targets.Add(T);
            }

            return P;
        }
    }
}
