using System;
using System.Collections.Generic;
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

        /// <summary>Путь к выбранному файлу-источнику (копируется при сохранении).</summary>
        public string PhotoSource;

        private IReadOnlyList<ControllerEntry> mControllers;
        public IReadOnlyList<ControllerEntry> Controllers { get => mControllers; private set => Set(ref mControllers, value); }

        private ControllerEntry mSelectedController;
        public ControllerEntry SelectedController
        {
            get => mSelectedController;
            set
            {
                if (Set(ref mSelectedController, value) && (value != null))
                {
                    Model.Device = value.Name;
                    Model.Type = value.Type;
                    Raise(nameof(Device));
                    Raise(nameof(Type));
                }
            }
        }

        public TargetEditVM(TargetEdit Model, IReadOnlyList<ControllerEntry> Controllers)
        {
            this.Model = Model;
            mControllers = Controllers;

            foreach (var S in Model.Steps) Steps.Add(new StepEditVM(S, this));

            SelectByName(Model.Device);
        }

        public string Name { get => Model.Name; set { Model.Name = value; Raise(); } }
        public string Device { get => Model.Device; set { Model.Device = value; Raise(); } }
        public string Type { get => Model.Type; set { Model.Type = value; Raise(); } }
        public string Photo { get => Model.Photo; set { Model.Photo = value; Raise(); } }
        public string Doc { get => Model.Doc; set { Model.Doc = value; Raise(); } }

        public void SetControllers(IReadOnlyList<ControllerEntry> Controllers)
        {
            mControllers = Controllers;
            Raise(nameof(Controllers));
            SelectByName(Model.Device);
        }

        private void SelectByName(string Name)
        {
            mSelectedController = null;

            if (Controllers != null)
                foreach (var C in Controllers)
                    if (C.Name == Name) { mSelectedController = C; break; }

            Raise(nameof(SelectedController));
        }

        public void SetPhoto(string Source)
        {
            if (string.IsNullOrEmpty(Source)) return;
            PhotoSource = Source;
            Photo = Path.GetFileName(Source);
        }

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

        /// <summary>Каталог контроллеров (для выбора в целях).</summary>
        public IReadOnlyList<ControllerEntry> Controllers { get; private set; } = new ControllerEntry[0];

        /// <summary>Путь к файлу каталога контроллеров.</summary>
        public string CatalogPath => AppPaths.Catalog(Session.Env);

        public ProjectEditorViewModel(Session Session, Project Project)
        {
            this.Session = Session;
            Original = Project;
            IsNew = Project == null;

            Controllers = ControllerCatalog.Load(AppPaths.Catalog(Session.Env)).Entries;

            Model = (Project != null)
                  ? ProjectEditorModel.FromProject(Project)
                  : new ProjectEditorModel { Date = DateTime.Now.ToString("yyyy-MM-dd") };

            foreach (var T in Model.Targets) Targets.Add(new TargetEditVM(T, Controllers));
            if (Targets.Count == 0) AddTarget();
        }

        /// <summary>Перечитать каталог контроллеров (после его редактирования).</summary>
        public void ReloadControllers()
        {
            Controllers = ControllerCatalog.Load(AppPaths.Catalog(Session.Env)).Entries;
            Raise(nameof(Controllers));

            foreach (var T in Targets) T.SetControllers(Controllers);
        }

        /// <summary>Путь к выбранному файлу-источнику фото проекта (копируется при сохранении).</summary>
        public string ProjectPhotoSource;

        public string Title => IsNew ? "Новый проект" : $"Проект: {Model.Name}";

        public string Name { get => Model.Name; set { Model.Name = value; Raise(); Raise(nameof(Title)); } }
        public string Description { get => Model.Description; set { Model.Description = value; Raise(); } }
        public string Date { get => Model.Date; set { Model.Date = value; Raise(); } }
        public string Type { get => Model.Type; set { Model.Type = value; Raise(); } }
        public string Photo { get => Model.Photo; set { Model.Photo = value; Raise(); } }
        public string Doc { get => Model.Doc; set { Model.Doc = value; Raise(); } }

        public void SetProjectPhoto(string Source)
        {
            if (string.IsNullOrEmpty(Source)) return;
            ProjectPhotoSource = Source;
            Photo = Path.GetFileName(Source);
        }

        public void AddTarget()
        {
            var T = new TargetEdit { Name = "Основной МК" };
            Model.Targets.Add(T);
            Targets.Add(new TargetEditVM(T, Controllers));
        }

        public void RemoveTarget(TargetEditVM Target)
        {
            if (Target == null) return;
            Model.Targets.Remove(Target.Model);
            Targets.Remove(Target);
        }

        public void Save()
        {
            var Root = (Session.Env.Conf.Projects.Count > 0) ? Session.Env.Conf.Projects[0] : "Projects";
            var Dir = IsNew ? Path.Combine(Root, Sanitize(Model.Name)) : Original.Dir;

            if (IsNew) Directory.CreateDirectory(Dir);

            // Копируем выбранные изображения в папку проекта
            if (!string.IsNullOrEmpty(ProjectPhotoSource))
                Model.Photo = CopyPhoto(ProjectPhotoSource, Dir);

            foreach (var TE in Targets)
                if (!string.IsNullOrEmpty(TE.PhotoSource))
                    TE.Model.Photo = CopyPhoto(TE.PhotoSource, Dir);

            var P = Model.ToProject();

            if (IsNew)
            {
                var Slug = Sanitize(Model.Name);
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

        private static string CopyPhoto(string Source, string Dir)
        {
            var Name = Path.GetFileName(Source);
            var Dest = Path.Combine(Dir, Name);

            try
            {
                if (!string.Equals(Path.GetFullPath(Source), Path.GetFullPath(Dest), StringComparison.OrdinalIgnoreCase))
                    File.Copy(Source, Dest, true);
            }
            catch { /* оставляем как есть */ }

            return Name;
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
