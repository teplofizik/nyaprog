using System.Collections.ObjectModel;

using NyaProg.App.Modern.Mvvm;
using Programmer.Project;

namespace NyaProg.App.Modern.ViewModels
{
    public sealed class ControllerEntryVM : Observable
    {
        public ControllerEntry Model { get; }

        public ControllerEntryVM(ControllerEntry Model) { this.Model = Model; }

        public string Name { get => Model.Name; set { Model.Name = value; Raise(); } }
        public string Type { get => Model.Type; set { Model.Type = value; Raise(); } }
    }

    /// <summary>
    /// Редактор каталога контроллеров (название + тип).
    /// </summary>
    public sealed class ControllersViewModel : Observable
    {
        private readonly string File;

        public ObservableCollection<ControllerEntryVM> Items { get; } = new ObservableCollection<ControllerEntryVM>();

        public ControllersViewModel(string File)
        {
            this.File = File;

            var C = ControllerCatalog.Load(File);
            foreach (var E in C.Entries) Items.Add(new ControllerEntryVM(E));
        }

        public void Add() => Items.Add(new ControllerEntryVM(new ControllerEntry("Новый контроллер", "stm32")));

        public void Remove(ControllerEntryVM Entry)
        {
            if (Entry != null) Items.Remove(Entry);
        }

        public void Save()
        {
            var C = new ControllerCatalog();
            foreach (var V in Items) C.Entries.Add(V.Model);
            C.Save(File);
        }
    }
}
