using System;
using System.Collections.Generic;

using Extension.Argument;
using Programmer.Input;
using Programmer.Modules;
using Programmer.Options;
using Programmer.Options.Types;

namespace NyaProg.Presentation
{
    public enum OptionKind { List, Text, Bool, Input, Dynamic, Hidden }

    /// <summary>
    /// UI-независимая модель опции: описание + текущее значение + аргументы.
    /// </summary>
    public sealed class OptionModel
    {
        public OptionKind Kind { get; private set; }

        public string Name { get; private set; }

        public string ID { get; private set; }

        public string ParamName { get; private set; }

        public bool Autoincrement { get; private set; }

        // Данные
        public IReadOnlyList<CommandItem> Items { get; private set; } = new CommandItem[0];

        public string BoolValue { get; private set; } = "1";

        public InputType Type { get; private set; }

        // Текущее значение
        public int SelectedIndex { get; set; }

        public string Text { get; set; } = "";

        public bool Checked { get; set; }

        public static OptionModel FromOption(Option O)
        {
            var M = new OptionModel { Name = O.Name, ID = O.ID };

            var OL = O as OptionList;
            if (OL != null)
            {
                M.Kind = OptionKind.List;
                var Items = new List<CommandItem>();
                foreach (var I in OL.Items) Items.Add(new CommandItem(I.Label, I.Values));
                M.Items = Items;
                M.SelectedIndex = DefaultListIndex(OL);
                return M;
            }

            var OD = O as OptionDynamic;
            if (OD != null)
            {
                M.Kind = OptionKind.Dynamic;
                M.Items = OD.Items ?? new CommandItem[0];
                M.SelectedIndex = DefaultDynamicIndex(OD);
                return M;
            }

            var OB = O as OptionBool;
            if (OB != null)
            {
                M.Kind = OptionKind.Bool;
                M.ParamName = OB.ParamName;
                M.BoolValue = OB.Value;
                M.Checked = OB.Default;
                return M;
            }

            var OI = O as OptionInput;
            if (OI != null)
            {
                M.Kind = OptionKind.Input;
                M.ParamName = OI.ParamName;
                M.Type = OI.Type;
                M.Autoincrement = OI.Autoincrement;
                M.Text = OI.Default;
                return M;
            }

            var OT = O as OptionText;
            if (OT != null)
            {
                M.Kind = OptionKind.Text;
                M.ParamName = OT.ParamName;
                M.Autoincrement = OT.Autoincrement;
                M.Text = OT.Default;
                return M;
            }

            var OH = O as OptionHidden;
            if (OH != null)
            {
                M.Kind = OptionKind.Hidden;
                M.ParamName = OH.ParamName;
                M.Text = OH.Default;
                return M;
            }

            M.Kind = OptionKind.Hidden;
            return M;
        }

        /// <summary>Аргументы, которые задаёт опция.</summary>
        public ArgumentList Args
        {
            get
            {
                var A = new ArgumentList();

                switch (Kind)
                {
                    case OptionKind.List:
                    case OptionKind.Dynamic:
                        if ((Items != null) && (Items.Count > 0))
                            A.Append(Items[Clamp(SelectedIndex, 0, Items.Count - 1)].Values);
                        break;
                    case OptionKind.Text:
                        A.Set(ParamName, Text ?? "");
                        break;
                    case OptionKind.Input:
                        A.Set(ParamName, (Type != null) ? Type.Convert(Text ?? "") : (Text ?? ""));
                        break;
                    case OptionKind.Bool:
                        A.Set(ParamName, Checked ? BoolValue : "");
                        break;
                    case OptionKind.Hidden:
                        A.Set(ParamName, Text ?? "");
                        break;
                }

                return A;
            }
        }

        public bool Increment()
        {
            if ((Kind == OptionKind.Input) && (Type != null) && Type.CanIncrement)
            {
                Text = Type.Increment(Text);
                return true;
            }

            return false;
        }

        /// <summary>Значение для сохранения в предпочтениях.</summary>
        public string Preference
        {
            get
            {
                switch (Kind)
                {
                    case OptionKind.List:
                    case OptionKind.Dynamic:
                        return ((Items != null) && (Items.Count > 0)) ? Items[Clamp(SelectedIndex, 0, Items.Count - 1)].Label : null;
                    case OptionKind.Bool:
                        return Checked ? "1" : "0";
                    default:
                        return Text;
                }
            }
        }

        public void SetPreference(string Value)
        {
            if (Value == null) return;

            switch (Kind)
            {
                case OptionKind.List:
                case OptionKind.Dynamic:
                    if (Items != null)
                        for (int i = 0; i < Items.Count; i++)
                            if (Items[i].Label == Value) { SelectedIndex = i; return; }
                    break;
                case OptionKind.Bool:
                    Checked = (Value != "0") && !string.Equals(Value, "false", StringComparison.OrdinalIgnoreCase);
                    break;
                case OptionKind.Input:
                    if ((Type == null) || Type.Check(Value)) Text = Value;
                    break;
                default:
                    Text = Value;
                    break;
            }
        }

        private static int DefaultListIndex(OptionList OL)
        {
            for (int i = 0; i < OL.Items.Length; i++)
                if (OL.Items[i].Default) return i;

            return 0;
        }

        private static int DefaultDynamicIndex(OptionDynamic OD)
        {
            if ((OD.Items == null) || (OD.Items.Count == 0)) return 0;

            if (OD.DefaultLabel != null)
                for (int i = 0; i < OD.Items.Count; i++)
                    if (OD.Items[i].Label == OD.DefaultLabel) return i;

            return 0;
        }

        private static int Clamp(int V, int Lo, int Hi) => (V < Lo) ? Lo : ((V > Hi) ? Hi : V);
    }
}
