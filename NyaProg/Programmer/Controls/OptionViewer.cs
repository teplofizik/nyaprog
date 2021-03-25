using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Programmer.Options;
using Programmer.Options.Types;
using Extension.Argument;

namespace Programmer.Controls
{
    public partial class OptionViewer : UserControl
    {
        public event EventHandler OnChange;

        private Option InternalOption;
        private Control EditControl;

        /// <summary>
        /// Выбранные аргументы
        /// </summary>
        public ArgumentList Args => GetResult();

        /// <summary>
        /// Корректные ли введённые значения
        /// TODO: проверка пользовательского ввода
        /// </summary>
        public bool Correct
        {
            get
            {
                if ((InternalOption != null) && (InternalOption.GetType() == typeof(OptionInput)))
                {
                    var OI = InternalOption as OptionInput;
                    if (OI.Type != null)
                        // Проверим
                        return OI.Type.Check((EditControl as TextBox).Text);
                    else
                        // Тип не поддерживается
                        return false;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// Опция
        /// </summary>
        public Option Option
        {
            get { return InternalOption; }
            set
            {
                InternalOption = value;
                SetupOption();
            }
        }

        /// <summary>
        /// Идентификатор опции (для сохранения)
        /// </summary>
        public string ID => (InternalOption != null) ? InternalOption.ID : null;

        /// <summary>
        /// Значение опции (для сохранения)
        /// </summary>
        public string Value => GetValue();

        public void Increment()
        {
            var O = InternalOption;
            if(O.GetType() == typeof(OptionInput))
            {

            }
        }

        private string GetValue()
        {
            var O = InternalOption;
            if (O != null)
            {
                if (O.GetType() == typeof(OptionText))
                    return (EditControl as TextBox).Text;
                if (O.GetType() == typeof(OptionInput))
                    return (EditControl as TextBox).Text;
                else if (O.GetType() == typeof(OptionList))
                    return (EditControl as ComboBox).Text;
            }
            return "";
        }

        private ArgumentList GetResult()
        {
            var AL = new ArgumentList();
            if (InternalOption != null)
            {
                var O = InternalOption;
                if (O.GetType() == typeof(OptionText))
                {
                    var OT = O as OptionText;
                    AL.Set(OT.ParamName, (EditControl as TextBox).Text);
                }
                if (O.GetType() == typeof(OptionInput))
                {
                    var OI = O as OptionInput;
                    AL.Set(OI.ParamName, OI.Type.Convert((EditControl as TextBox).Text));
                }
                else if (O.GetType() == typeof(OptionList))
                {
                    var OL = O as OptionList;
                    var CB = (EditControl as ComboBox);
                    int Index = CB.SelectedIndex;

                    AL.Append(OL.Items[Index].Values);
                }
            }
            return AL;
        }

        public void SetDefault(string Value)
        {
            var O = InternalOption;
            if (O != null)
            {
                if (O.GetType() == typeof(OptionText))
                {
                    var OT = O as OptionText;
                    (EditControl as TextBox).Text = (Value != null) ? Value : OT.Default;
                }
                if (O.GetType() == typeof(OptionInput))
                {
                    var OI = O as OptionInput;
                    (EditControl as TextBox).Text = ((Value != null) && OI.Type.Check(Value)) ? Value : OI.Default;
                }
                else if (O.GetType() == typeof(OptionList))
                {
                    var OL = O as OptionList;
                    var CB = (EditControl as ComboBox);
                    // попробуем установить значение снаружи
                    if(Value != null)
                    {
                        for(int i = 0; i < OL.Items.Length; i++)
                        {
                            var OLI = OL.Items[i];
                            if(OLI.Label.CompareTo(Value) == 0)
                            {
                                CB.SelectedIndex = i;
                                return;
                            }
                        }
                    }
                    // Не вышло. Посмотрим опцию default
                    for (int i = 0; i < OL.Items.Length; i++)
                    {
                        var OLI = OL.Items[i];
                        if (OLI.Default)
                        {
                            CB.SelectedIndex = i;
                            return;
                        }
                    }

                    // Не было иных указаний - выбираем первый
                    if (OL.Items.Length > 0)
                        CB.SelectedIndex = 0;
                }
            }
        }

        private void SetupOption()
        {
            ComponentPlacer.CleanContainer(this);
            EditControl = null;
            if (InternalOption != null)
            {
                var O = InternalOption;

                if(O.GetType() == typeof(OptionText))
                {
                    var OT = O as OptionText;
                    EditControl = ComponentPlacer.PlaceText(this, OT.Default, O);
                }
                else if (O.GetType() == typeof(OptionList))
                {
                    var OL = O as OptionList;
                    EditControl = ComponentPlacer.PlaceComboBox(this, OL.Items, O);
                    (EditControl as ComboBox).SelectedIndexChanged += OptionViewer_SelectedIndexChanged;
                }
                else if(O.GetType() == typeof(OptionInput))
                {
                    var OI = O as OptionInput;
                    EditControl = ComponentPlacer.PlaceText(this, OI.Default, O);
                    EditControl.Width -= EditControl.Height;
                    ComponentPlacer.PlaceButton(this, 
                                                EditControl, 
                                                EditControl.Width, 
                                                0, 
                                                EditControl.Height, 
                                                EditControl.Height, 
                                                "+", 
                                                EC =>
                                                      {
                                                          EditControl.Text = OI.Type.Increment(EditControl.Text);

                                                          OnChange?.Invoke(this, new EventArgs());
                                                      });
                }
            }
        }

        private void OptionViewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnChange?.Invoke(this, new EventArgs());
        }

        public OptionViewer()
        {
            InitializeComponent();
        }

        private void OptionViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
