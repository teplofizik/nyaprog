using System;
using System.Collections.ObjectModel;
using System.IO;

using NyaProg.App.Modern.Mvvm;
using NyaProg.Presentation;
using Programmer.Project;

namespace NyaProg.App.Modern.ViewModels
{
    public sealed class StepEditVM : Observable
    {
        public StepEdit Model { get; }
        public TargetEditVM Owner { get; }

        public StepEditVM(StepEdit Model, TargetEditVM Owner)
        {
            this.Model = Model;
            this.Owner = Owner;
        }

        public string Command { get => Model.Command; set { Model.Command = value; Raise(); } }
        public string Value { get => Model.Value; set { Model.Value = value; Raise(); } }
        public string Comment { get => Model.Comment; set { Model.Comment = value; Raise(); } }
    }

    public sealed class TargetEditVM : Observable
    {
        public TargetEdit Model { get; }
        public ObservableCollection<StepEditVM> Steps { get; } = new ObservableCollection<StepEditVM>();

        public TargetEditVM(TargetEdit Model)
        {
            this.Model = Model;
            foreach (var S in Model.Steps) Steps.Add(new StepEditVM(S, this));
        }

        public string Name { get => Model.Name; set { Model.Name = value; Raise(); } }
        public string Device { get => Model.Device; set { Model.Device = value; Raise(); } }
        public string Photo { get => Model.Photo; set { Model.Photo = value; Raise(); } }

        public void AddStep()
        {
            var S = new StepEdit();
            Model.Steps.Add(S);
            Steps.Add(new StepEditVM(S, this));
        }

        public void RemoveStep(StepEditVM Step)
        {
            if (Step == null) return;
            Model.Steps.Remove(Step.Model);
            Steps.Remove(Step);
        }
    }

    /// <summary>
    /// Редактор проекта: создание и простое редактирование.
    /// </summary>
    public sealed class ProjectEditorViewModel : Observable
    {
        private readonly Session Session;
        private readonly Project Original;

        public ProjectEditorModel Model { get; }
        public ObservableCollection<TargetEditVM> Targets { get; } = new ObservableCollection<TargetEditVM>();

        public bool IsNew { get; }
        public bool Saved { get; private set; }
        public Project SavedProject { get; private set; }

        public ProjectEditorViewModel(Session Session, Project Project)
        {
            this.Session = Session;
            Original = Project;
            IsNew = Project == null;

            Model = (Project != null)
                  ? ProjectEditorModel.FromProject(Project)
                  : new ProjectEditorModel { Date = DateTime.Now.ToString("yyyy-MM-dd") };

            foreach (var T in Model.Targets) Targets.Add(new TargetEditVM(T));
            if (Targets.Count == 0) AddTarget();
        }

        public string Title => IsNew ? "Новый проект" : $"Проект: {Model.Name}";

        public string Name { get => Model.Name; set { Model.Name = value; Raise(); Raise(nameof(Title)); } }
        public string Description { get => Model.Description; set { Model.Description = value; Raise(); } }
        public string Date { get => Model.Date; set { Model.Date = value; Raise(); } }
        public string Type { get => Model.Type; set { Model.Type = value; Raise(); } }
        public string Photo { get => Model.Photo; set { Model.Photo = value; Raise(); } }

        public void AddTarget()
        {
            var T = new TargetEdit { Name = "Основной МК" };
            Model.Targets.Add(T);
            Targets.Add(new TargetEditVM(T));
        }

        public void RemoveTarget(TargetEditVM Target)
        {
            if (Target == null) return;
            Model.Targets.Remove(Target.Model);
            Targets.Remove(Target);
        }

        public void Save()
        {
            var P = Model.ToProject();

            if (IsNew)
            {
                var Root = (Session.Env.Conf.Projects.Count > 0) ? Session.Env.Conf.Projects[0] : "Projects";
                var Slug = Sanitize(Model.Name);
                var Dir = Path.Combine(Root, Slug);
                Directory.CreateDirectory(Dir);

                var FileName = Path.Combine(Dir, Slug + ".nyap");
                P.File = FileName;
                P.Dir = Dir;

                Programmer.Project.XML.XMLWriter.Save(P, FileName);
                Session.Env.AddProject(P);
                SavedProject = P;
            }
            else
            {
                Programmer.Project.XML.XMLWriter.Save(P, Original.File);
                Programmer.Project.XML.XMLLoader.Reload(Original);
                SavedProject = Original;
            }

            Saved = true;
        }

        private static string Sanitize(string Name)
        {
            var S = string.IsNullOrWhiteSpace(Name) ? "project" : Name.Trim();

            foreach (var C in Path.GetInvalidFileNameChars())
                S = S.Replace(C, '_');

            return S;
        }
    }
}
