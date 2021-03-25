using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Programmer.Controls.Mappers;
using Programmer.Options.Types;
using Programmer.Project;

namespace Programmer.Controls
{
    /// <summary>
    /// Размещение генерируемых компонентов 
    /// </summary>
    public static class ComponentPlacer
    {
        public static void HideTabs(TabControl TC)
        {
            TC.Appearance = TabAppearance.FlatButtons;
            TC.ItemSize = new Size(0, 1);
            TC.SizeMode = TabSizeMode.Fixed;
        }

        public static void CleanContainer(Control Base)
        {
            Base.Controls.Clear();
        }

        public static void Remove(Control Base, Control[] Controls)
        {
            foreach (var C in Controls)
                Base.Controls.Remove(C);
        }


        /// <summary>
        /// Разместить опции
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Actions"></param>
        /// <param name="M"></param>
        /// <returns></returns>
        public static OptionViewer[] PlaceOptions(Control Base, Options.Option[] Options, OptionMapper M, Action<Options.Option> OnChange)
        {
            var Opts = new List<OptionViewer>();
            M.Reset();
            foreach (var O in Options)
            {
                if (O.GetType() != typeof(OptionHidden))
                {
                    var OV = M.CreateNextControl("", O) as OptionViewer;
                    OV.Option = O;

                   // if(O.GetType() == typeof(OptionList))
                        OV.OnChange += new EventHandler(delegate (Object o, EventArgs a)
                        {
                            var Option = (o as OptionViewer).Option;
                            OnChange(Option);
                        });

                    Opts.Add(OV);
                    Base.Controls.Add(OV);
                }
            }

            return Opts.ToArray();
        }

        /// <summary>
        /// Разместить этапы программирования
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Actions"></param>
        /// <param name="M"></param>
        /// <returns></returns>
        public static StepControl[] PlaceSteps(Control Base, Project.Action[] Actions, StepMapper M)
        {
            var Steps = new List<StepControl>();
            M.Reset();
            foreach (var A in Actions)
            {
                var S = M.CreateNextControl(A.Comment, A) as StepControl;
                Steps.Add(S);
                Base.Controls.Add(S);
            }

            return Steps.ToArray();
        }

        /// <summary>
        /// Размещение кнопок выбора скрипта
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Scripts"></param>
        /// <param name="M"></param>
        /// <param name="OnClick"></param>
        /// <returns></returns>
        public static Button[] PlaceScripts(Control Base, Script[] Scripts, ButtonMapper M, Action<Script> OnClick)
        {
            var Buttons = new List<Button>();
            M.Reset();
            foreach(var S in Scripts)
            {
                var B = M.CreateNextControl(S.Name, S) as Button;
                B.Click += new EventHandler(delegate (Object o, EventArgs a)
                {
                    var BS = (o as Button).Tag as Script;
                    OnClick(BS);
                });

                Buttons.Add(B);
                Base.Controls.Add(B);
            }

            return Buttons.ToArray();
        }

        /// <summary>
        /// Размещение кнопки
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Scripts"></param>
        /// <param name="M"></param>
        /// <param name="OnClick"></param>
        /// <returns></returns>
        public static Button PlaceButton(Control Base, object Tag, int L, int T, int W, int H, string Text, Action<object> OnClick)
        {
            var B = new Button();
            B.Left = L;
            B.Top = T;
            B.Width = W;
            B.Height = H;
            B.Tag = Tag;
            B.Text = Text;
            B.Click += new EventHandler(delegate (Object o, EventArgs a)  { OnClick((o as Button).Tag); });
            Base.Controls.Add(B);

            return B;
        }

        public static TextBox PlaceReadme(Control Base, string Text, int Left, int Width, int Bottom, int Height)
        {
            var TB = new TextBox();

            TB.AutoSize = false;
            TB.TabStop = false;
            TB.ScrollBars = ScrollBars.Vertical;
            TB.Multiline = true;
            TB.ReadOnly = true;
            TB.Width = Width;
            TB.Height = Height;
            TB.Left = Left;
            TB.Top = Base.Height - Height - Bottom;
            TB.Text = Text;
            Base.Controls.Add(TB);
            return TB;
        }

        public static Label PlaceLabel(Control Base, string Text, int Bottom, int Height)
        {
            var L = new Label();

            L.AutoSize = false;
            L.Width = Base.Width - 18;
            L.Height = Height;
            L.Left = 9;
            L.Top = Base.Height - Height - Bottom;
            L.Text = Text;
            Base.Controls.Add(L);
            return L;
        }

        public static TextBox PlaceText(Control Base, string Text, object Tag)
        {
            var TB = new TextBox();
            
            TB.Width = Base.Width;
            TB.Height = Base.Height;
            TB.Left = 0;
            TB.Top = 0;
            TB.Text = Text;
            TB.Tag = Tag;
            Base.Controls.Add(TB);
            return TB;
        }

        public static ComboBox PlaceComboBox(Control Base, object[] Items, object Tag, int L, int T, int W, int H)
        {
            var CB = new ComboBox();

            CB.DropDownStyle = ComboBoxStyle.DropDownList;
            CB.Width = W;
            CB.Height = H;
            CB.Left = L;
            CB.Top = T;
            CB.Items.AddRange(Items);
            if (Items.Length > 0) CB.SelectedIndex = 0;
            CB.Tag = Tag;

            Base.Controls.Add(CB);
            return CB;
        }

        public static ComboBox PlaceComboBox(Control Base, object[] Items, object Tag)
        {
            return PlaceComboBox(Base, Items, Tag, 0, 0, Base.Width, Base.Height);
        }
    }
}
