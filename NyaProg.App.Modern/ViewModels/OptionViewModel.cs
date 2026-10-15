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

        /// <summary>Выбранный пункт (для привязки ComboBox — надёжнее SelectedIndex).</summary>
        public CommandItem SelectedItem
        {
            get
            {
                if ((Model.Items == null) || (Model.SelectedIndex < 0) || (Model.SelectedIndex >= Model.Items.Count))
                    return null;

                return Model.Items[Model.SelectedIndex];
            }
            set
            {
                var Idx = -1;
                if ((value != null) && (Model.Items != null))
                    for (int i = 0; i < Model.Items.Count; i++)
                        if (ReferenceEquals(Model.Items[i], value)) { Idx = i; break; }

                if (Idx >= 0) SelectedIndex = Idx;
            }
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
