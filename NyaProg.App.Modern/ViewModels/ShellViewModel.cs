using System.Collections.ObjectModel;
using System.Windows.Input;

using NyaProg.App.Modern.Mvvm;
using NyaProg.Presentation;
using Programmer.Environment;

namespace NyaProg.App.Modern.ViewModels
{
    public enum ScreenKind { List, Targets, Flash }

    public sealed class ShellViewModel : Observable
    {
        public Session Session { get; }

        public ObservableCollection<ProjectCard> Projects { get; } = new ObservableCollection<ProjectCard>();
        public ObservableCollection<TargetCard> Targets { get; } = new ObservableCollection<TargetCard>();

        private string mSearch = "";
        public string Search
        {
            get => mSearch;
            set { if (Set(ref mSearch, value)) RefreshProjects(); }
        }

        private ScreenKind mScreen = ScreenKind.List;
        public ScreenKind Screen
        {
            get => mScreen;
            private set
            {
                if (Set(ref mScreen, value))
                {
                    Raise(nameof(IsList));
                    Raise(nameof(IsTargets));
                    Raise(nameof(IsFlash));
                }
            }
        }

        public bool IsList => Screen == ScreenKind.List;
        public bool IsTargets => Screen == ScreenKind.Targets;
        public bool IsFlash => Screen == ScreenKind.Flash;

        private ProjectCard mProject;
        public ProjectCard Project
        {
            get => mProject;
            private set
            {
                if (Set(ref mProject, value))
                {
                    Raise(nameof(HasProject));
                    Raise(nameof(ProjectName));
                    Raise(nameof(CanCrumbProject));
                }
            }
        }

        public bool HasProject => Project != null;
        public string ProjectName => (Project != null) ? Project.Name : "";

        private TargetCard mTarget;
        public TargetCard Target
        {
            get => mTarget;
            private set
            {
                if (Set(ref mTarget, value))
                {
                    Raise(nameof(HasTarget));
                    Raise(nameof(TargetName));
                }
            }
        }

        public bool HasTarget => Target != null;
        public string TargetName => (Target != null) ? Target.Name : "";

        private FlashViewModel mFlash;
        public FlashViewModel Flash { get => mFlash; private set => Set(ref mFlash, value); }

        public bool CanCrumbProject => Targets.Count > 1;

        public ICommand OpenProjectCommand { get; }
        public ICommand OpenTargetCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand CrumbProjectCommand { get; }

        public ShellViewModel(ProgEnv Env)
        {
            Session = new Session(Env);

            OpenProjectCommand = new RelayCommand(P => OpenProject((ProjectCard)P));
            OpenTargetCommand = new RelayCommand(P => OpenTarget((TargetCard)P));
            HomeCommand = new RelayCommand(P => Home());
            CrumbProjectCommand = new RelayCommand(P => BackToTargets());

            RefreshProjects();
        }

        private void RefreshProjects()
        {
            Session.Projects.Search = Search;

            Projects.Clear();
            foreach (var C in Session.Projects.Items()) Projects.Add(C);
        }

        public void OpenProject(ProjectCard C)
        {
            Session.Open(C.Project);
            Project = C;
            Target = null;

            Targets.Clear();
            foreach (var T in Session.Targets.Items()) Targets.Add(T);
            Raise(nameof(CanCrumbProject));

            if (Session.Targets.Skip && (Targets.Count == 1))
                OpenTarget(Targets[0]);
            else
                Screen = ScreenKind.Targets;
        }

        public void OpenTarget(TargetCard C)
        {
            Session.Open(C.Target);
            Target = C;

            Flash = new FlashViewModel(Session.Flash);
            Screen = ScreenKind.Flash;
        }

        private void BackToTargets()
        {
            if (Project == null) { Home(); return; }
            OpenProject(Project);
        }

        private void Home()
        {
            Session.Home();

            Project = null;
            Target = null;
            Flash = null;

            Screen = ScreenKind.List;
        }
    }
}
