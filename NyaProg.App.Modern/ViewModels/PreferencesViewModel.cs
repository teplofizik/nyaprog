using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using NyaProg.App.Modern.Mvvm;
using NyaProg.Presentation;
using Programmer.Tool;

namespace NyaProg.App.Modern.ViewModels
{
    public sealed class ArchPrefItem : Observable
    {
        public string Arch { get; }

        public IReadOnlyList<UniTool> Programmers { get; }

        private UniTool mSelected;
        public UniTool Selected { get => mSelected; set => Set(ref mSelected, value); }

        public ArchPrefItem(string Arch, IReadOnlyList<UniTool> Programmers, UniTool Selected)
        {
            this.Arch = Arch;
            this.Programmers = Programmers;
            mSelected = Selected;
        }
    }

    /// <summary>
    /// Настройки: программатор по умолчанию для каждого типа контроллера.
    /// </summary>
    public sealed class PreferencesViewModel : Observable
    {
        private readonly Session Session;

        public ObservableCollection<ArchPrefItem> Items { get; } = new ObservableCollection<ArchPrefItem>();

        public ICommand SaveCommand { get; }

        private string mStatus = "";
        public string Status { get => mStatus; private set => Set(ref mStatus, value); }

        public PreferencesViewModel(Session Session)
        {
            this.Session = Session;
            SaveCommand = new RelayCommand(P => Save());
            Build();
        }

        private void Build()
        {
            var Archs = new SortedSet<string>();

            foreach (var P in Session.Env.Projects)
                foreach (var A in P.Arch) Archs.Add(A);

            foreach (var T in Session.Env.Tools)
                foreach (var A in T.GetSupportedArch()) Archs.Add(A);

            foreach (var A in Archs)
            {
                var Tools = Session.Env.GetToolsByArch(new[] { A });
                var Cur = Session.Env.GetPreference(Prefs.ArchProgrammer(A));
                var Sel = (Cur != null) ? Tools.FirstOrDefault(T => T.Name == Cur) : null;
                if ((Sel == null) && (Tools.Length > 0)) Sel = Tools[0];

                Items.Add(new ArchPrefItem(A, Tools, Sel));
            }
        }

        private void Save()
        {
            foreach (var I in Items)
                Session.Env.SetPreference(Prefs.ArchProgrammer(I.Arch), I.Selected?.Name ?? "");

            Session.Env.Save(Session.Env.Conf.Preferences);

            Status = "Сохранено";
        }
    }
}
