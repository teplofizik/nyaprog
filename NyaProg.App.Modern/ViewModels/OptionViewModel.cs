using System.Collections.Generic;

using NyaProg.App.Modern.Mvvm;
using NyaProg.Presentation;
using Programmer.Modules;

namespace NyaProg.App.Modern.ViewModels
{
    public sealed class OptionViewModel : Observable
    {
        public OptionModel Model { get; }

        private readonly System.Action Changed;

        public OptionViewModel(OptionModel Model, System.Action Changed)
        {
            this.Model = Model;
            this.Changed = Changed;
        }

        public string Name => Model.Name;

        public bool IsVisible => Model.Kind != OptionKind.Hidden;
        public bool IsList => (Model.Kind == OptionKind.List) || (Model.Kind == OptionKind.Dynamic);
        public bool IsBool => Model.Kind == OptionKind.Bool;
        public bool IsText => (Model.Kind == OptionKind.Text) || (Model.Kind == OptionKind.Input);

        public IReadOnlyList<CommandItem> Items => Model.Items;

        public int SelectedIndex
        {
            get => Model.SelectedIndex;
            set { if (Model.SelectedIndex != value) { Model.SelectedIndex = value; Changed?.Invoke(); } }
        }

        public bool Checked
        {
            get => Model.Checked;
            set { if (Model.Checked != value) { Model.Checked = value; Changed?.Invoke(); } }
        }

        public string Text
        {
            get => Model.Text;
            set { if (Model.Text != value) { Model.Text = value; Changed?.Invoke(); } }
        }
    }
}
