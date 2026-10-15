using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Avalonia.Threading;

using NyaProg.App.Modern.Mvvm;
using NyaProg.Presentation;
using Programmer.Tool;

namespace NyaProg.App.Modern.ViewModels
{
    public sealed class FlashViewModel : Observable
    {
        public FlashModel Model { get; }

        public ObservableCollection<OptionViewModel> Options { get; } = new ObservableCollection<OptionViewModel>();
        public ObservableCollection<StepViewModel> Steps { get; } = new ObservableCollection<StepViewModel>();

        public IReadOnlyList<UniTool> Programmers => Model.Programmers;

        private UniTool mProgrammer;
        public UniTool Programmer
        {
            get => mProgrammer;
            set
            {
                if (Set(ref mProgrammer, value) && (value != null))
                    Model.SelectProgrammer(value);
            }
        }

        public string Title => (Model.Target != null) ? Model.Target.Name : "";
        public string Status => Model.Status;
        public bool Running => Model.Running;
        public string PhotoPath => Model.PhotoPath;
        public string Readme => Model.Readme;

        public ICommand StartCommand { get; }
        public ICommand CancelCommand { get; }

        public FlashViewModel(FlashModel Model)
        {
            this.Model = Model;
            mProgrammer = Model.Programmer;

            StartCommand = new RelayCommand(P => Model.Start(), P => !Model.Running);
            CancelCommand = new RelayCommand(P => Model.Cancel(), P => Model.Running);

            Model.Changed += OnChanged;
            Refresh();
        }

        private void OnChanged() => Dispatcher.UIThread.Post(Refresh);

        private void Refresh()
        {
            Raise(nameof(Programmers));
            Raise(nameof(Programmer));
            Raise(nameof(Title));
            Raise(nameof(Status));
            Raise(nameof(Running));
            Raise(nameof(PhotoPath));
            Raise(nameof(Readme));

            if (!SameOptions())
            {
                Options.Clear();
                foreach (var O in Model.Options) Options.Add(new OptionViewModel(O, OnOptionChanged));
            }

            Steps.Clear();
            foreach (var S in Model.Steps) Steps.Add(new StepViewModel(S));
        }

        private bool SameOptions()
        {
            if (Options.Count != Model.Options.Count) return false;

            for (int i = 0; i < Options.Count; i++)
                if (!ReferenceEquals(Options[i].Model, Model.Options[i])) return false;

            return true;
        }

        private void OnOptionChanged()
        {
            Model.ApplyOptions();
            Model.RebuildPlan();
        }
    }
}
