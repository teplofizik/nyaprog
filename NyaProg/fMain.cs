using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Programmer;
using Programmer.Controls;
using Programmer.Environment;
using Programmer.Project;
using Programmer.Controls.Mappers;
using Programmer.Tool;
using Programmer.Options;
using Programmer.Options.Types;
using NyaProg.Hosting;
using System.IO;
using System.Security.Cryptography;

namespace NyaProg
{
    // https://www.codeproject.com/articles/258681/windows-forms-modular-app-using-mef - MEF
    public partial class fMain : Form
    {
        State PrState;
        StepControl[] StepsControls;
        OptionViewer[] OptionsControls;
        ComboBox ToolSelector;
        TextBox ProjReadme;
        TextBox ScriptReadme;


        public fMain()
        {
            InitializeComponent();
        }

        private void Test()
        {
            var E = PrState.Env;

            var P = E.Projects[0];
            var Script = P.Scripts[1];
            var SE = new ScriptExec();
            var Arch = SE.GetScriptArch(P, Script);
            var Tools = E.GetToolsByArch(Arch);
            var Tool = Tools[0];
            var Options = new Extension.Argument.ArgumentList();
            Options.Set("input", "123456789");
            Options.Append(Tool.GetDefaultOptionsValues());

            foreach (var A in Script.Steps.OfType<Programmer.Project.Action>())
            {
                var Res = SE.Exec(E, Tool, P, Script, A, Options);
                if (Res.Result == ExecResultType.Error)
                {
                    Log.WriteLine($"Error: {Res.Message}");
                    break;
                }
            }
        }

        private void UpdateProjList()
        {
            cbProject.BeginUpdate();
            cbProject.Items.Clear();
            cbProject.Items.AddRange(PrState.Env.Projects);
            cbProject.EndUpdate();

            if (PrState.Env.Projects.Length > 0) cbProject.SelectedIndex = 0;
        }

        private void ShowTools(UniTool[] Tools)
        {
            ToolSelector = ComponentPlacer.PlaceComboBox(gbOptions, Tools, null, 15, 25, 220, 22);

            var Pref = PrState.Env.GetPreference(PrState.Project.GetID(PrState.Script));
            PrState.Tool = null;
            if (Pref != null)
            {
                for(int i = 0; i < ToolSelector.Items.Count; i++)
                {
                    if ((ToolSelector.Items[i] as UniTool).Name.CompareTo(Pref) == 0)
                    {
                        ToolSelector.SelectedIndex = i;
                        PrState.Tool = ToolSelector.SelectedItem as UniTool;
                        break;
                    }
                }
            }

            if ((PrState.Tool == null) && (Tools.Length > 0))
            {
                ToolSelector.SelectedIndex = 0;
                PrState.Tool = ToolSelector.SelectedItem as UniTool;
            }

            ToolSelector.SelectedIndexChanged += ToolSelector_SelectedIndexChanged;
        }

        private void ToolSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrState.Tool = ToolSelector.SelectedItem as UniTool;

            ShowOptions();
            ApplyOptions();
            RebuildSteps();
        }

        private void OnOptionChange(Option O)
        {
            ApplyOptions();
            RebuildSteps();
        }

        private void ShowOptions()
        {
            var OptsTool = new List<Option>();
            var OptsProj = new List<Option>();
            var OptViewers = new List<OptionViewer>();

            if (PrState.Tool != null) OptsTool.AddRange(PrState.Tool.GetOptions());
            if (PrState.Project != null) OptsProj.AddRange(PrState.Project.Options);
            if (PrState.Script != null)
            {
                OptsProj.AddRange(PrState.Script.Options);

                var Input = PrState.Script.Input;
                var IL = PrState.Env.InputTypes;
                foreach(var I in Input)
                    OptsProj.Add(new OptionInput(I.Name, I.Name, IL.GetInputType(I.Type), I.Default, I.ID, I.Autoincrement));
            }

            if (OptionsControls != null)
                ComponentPlacer.Remove(gbOptions, OptionsControls);

            // Динамические списки — подгружаем значения у команд-поставщиков
            var DynArgs = PrState.CompiledArgs ?? new Extension.Argument.ArgumentList();
            foreach (var O in OptsTool.Concat(OptsProj).OfType<OptionDynamic>())
                O.Items = PrState.Env.GetListItems(O.Source, DynArgs);

            OptViewers.AddRange(ComponentPlacer.PlaceOptions(gbOptions, OptsTool.ToArray(), new OptionMapper(15, 25+22+8, 220), OnOptionChange));
            OptViewers.AddRange(ComponentPlacer.PlaceOptions(gbOptions, OptsProj.ToArray(), new OptionMapper(gbOptions.Width - 208, 25, 200), OnOptionChange));

            foreach (var O in OptViewers)
            {
                if(O.ID != null) O.SetDefault(PrState.Env.GetPreference(O.ID));
            }

            // Кнопка обновления у динамических опций
            foreach (var V in OptViewers)
            {
                var OD = V.Option as OptionDynamic;
                if (OD != null)
                    V.RefreshItems = () => V.SetItems(PrState.Env.GetListItems(OD.Source, PrState.CompiledArgs ?? new Extension.Argument.ArgumentList()));
            }

            OptionsControls = OptViewers.ToArray();
        }

        private void SaveOptions()
        {
            // Сохраним выбор программатора для проекта
            if (PrState.Project != null)
                PrState.Env.SetPreference(PrState.Project.GetID(PrState.Script), PrState.Tool.Name);
            if (OptionsControls != null)
            {
                foreach (var OC in OptionsControls)
                {
                    if (OC.ID != null) PrState.Env.SetPreference(OC.ID, OC.Value);
                }
            }

            SaveIncrementFields();
        }

        private void ApplyOptions()
        {
            PrState.ResetArguments();

            if (OptionsControls != null)
            {
                foreach (var OC in OptionsControls)
                    PrState.CompiledArgs.Append(OC.Args);
            }
        }

        private void RebuildSteps()
        {
            ComponentPlacer.CleanContainer(gbSteps);

            var S = PrState.Script;
            if (S != null)
            {
                PrState.Plan = S.GetSteps(PrState);

                StepsControls = ComponentPlacer.PlaceSteps(gbSteps,
                                                           PrState.Plan.ToArray(),
                                                           new StepMapper(9, 25));
            }
        }

        private void SelectScript(Script S)
        {
            if(OptionsControls != null)
                ComponentPlacer.Remove(gbOptions, OptionsControls);
            if (ScriptReadme != null)
            {
                ComponentPlacer.Remove(gbOptions, new Control[] { ScriptReadme });
                ScriptReadme = null;
            }
            if (ToolSelector != null)
            {
                ToolSelector.SelectedIndexChanged -= ToolSelector_SelectedIndexChanged;
                ComponentPlacer.Remove(gbOptions, new Control[] { ToolSelector });
            }
            PrState.SelectScript(S);
            lResult.Text = "";

            if (S != null)
            {
                var Tools = PrState.Env.GetToolsByArch(PrState.Arch);
                ShowTools(Tools);
                ShowOptions();
                ApplyOptions();
                RebuildSteps();
                InitIncrementFields();

                if (S.Readme != null)
                    ScriptReadme = ComponentPlacer.PlaceReadme(gbOptions, S.Readme, 9, gbOptions.Width - 18, 58, 180);
                else if (PrState.Project.Readme != null)
                    ScriptReadme = ComponentPlacer.PlaceReadme(gbOptions, PrState.Project.Readme, 9, gbOptions.Width - 18, 58, 180);

                gbOptions.Text = S.Name;
                tTabs.SelectedTab = tOptions;
            }
            else
            {
                RebuildSteps();
                InitIncrementFields();
                OptionsControls = null;
                ToolSelector = null;
                StepsControls = null;
                tTabs.SelectedTab = tScripts;
            }
        }

        private void UpdateScripts()
        {
            ComponentPlacer.CleanContainer(gbScripts);
            ComponentPlacer.PlaceScripts(gbScripts,
                                         PrState.Project.Scripts.ToArray(),
                                         new ButtonMapper(9, 25),
                                         S => SelectScript(S));
        }

        private void InitProgrammer()
        {
            PrState = new State("config.xml", new WinFormsUserInterface());
            Log.LogFileName = PrState.Env.Conf.Log;

            PluginHost.Load(PrState.Env, PrState.Env.Conf.Plugins);

            PrState.ActionCompleted += PrState_ActionCompleted;
            UpdateProjList();
        }

        private void InitDirs()
        {
            if (!Directory.Exists("Base")) Directory.CreateDirectory("Base");
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            ComponentPlacer.HideTabs(tTabs);
            InitProgrammer();
            InitDirs();
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            if(ProjReadme != null)
            {
                ComponentPlacer.Remove(gbOptions, new Control[] { ProjReadme });
                ProjReadme = null;
            }
            PrState.SelectProject(cbProject.SelectedItem as Project);
            Text = $"NyaProg - {PrState.Project.Name}. {PrState.Project.Description}";
            // Если скрипт один, то откроем его. 
            UpdateScripts();
            if (PrState.Project.Scripts.Count == 1)
                SelectScript(PrState.Project.Scripts[0]);
            else
                SelectScript(null);

            if(PrState.Project.Readme != null)
                ProjReadme = ComponentPlacer.PlaceReadme(gbScripts, PrState.Project.Readme, 9, gbScripts.Width - 18, 9, 220);
        }
        
        private void bBack_Click(object sender, EventArgs e)
        {
            SelectScript(null);
        }

        /// <summary>
        /// Этап прошивки завершен
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrState_ActionCompleted(object sender, ActionEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
           {
               SetStepResult(e.Action, false, e.Result);
               if (e.Result.Result == ExecResultType.Error)
               {
                   SetStatus(Color.Red, e.Result.Message);
                   Completed(false);
                   return;
               }
               else if (e.Result.Result == ExecResultType.Warning)
                   SetStatus(Color.Yellow, e.Result.Message);

                if (PrState.Action < PrState.Plan.Count - 1)
               {
                   if (PrState.Cancelled)
                   {
                       PrState.Cancelled = false;
                       SetStatus(Color.Red, "Отменено");
                       Completed(false);
                   }
                   else
                       StartStep(e.Action + 1);
               }
               else
               {
                   SetStatus(Color.Green, "Успешно выполнено!");
                   Completed(true);
               }
           });
        }

        /// <summary>
        /// Установить статус прошивки
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Text"></param>
        private void SetStepResult(int Index, bool Process, ExecResult R)
        {
            if ((StepsControls == null) || (Index < 0) || (Index >= PrState.Plan.Count)) return;

            var Target = PrState.Plan[Index];
            foreach (var S in StepsControls)
            {
                if (ReferenceEquals(S.Tag, Target))
                {
                    S.Process = Process;
                    S.Result = R;
                    S.Invalidate();
                }
            }
        }

        /// <summary>
        /// Установить статус прошивки
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Text"></param>
        private void SetStatus(Color C, string Text)
        {
            lResult.ForeColor = C;
            lResult.Text = Text;
        }

        private void Autoincrement()
        {
            if (OptionsControls != null)
            {
                foreach (var O in OptionsControls)
                {
                    if(O.Increment())
                    {
                        if(O.ID != null)
                            PrState.Env.SetPreference(O.ID, O.Value);
                    }
                }
            }
        }

        private bool HasIncrementableFields()
        {
            foreach (var OC in OptionsControls)
            {
                if (OC.Option.Autoincrement) return true;
            }

            return false;
        }

        private void SaveIncrementFields()
        {
            var PID = PrState.Project.GetID(PrState.Script);

            PrState.Env.SetPreference($"{PID}:autoinc", ckAutoinc.Checked ? "enabled" : "disabled");
            PrState.Env.SetPreference($"{PID}:success", ckSuccess.Checked ? "enabled" : "disabled");
        }

        private void InitIncrementFields()
        {
            var PID = PrState.Project.GetID(PrState.Script);

            if (HasIncrementableFields())
            {
                ckAutoinc.Visible = true;
                ckSuccess.Visible = true;
                ckAutoinc.Checked = PrState.Env.GetPreference($"{PID}:autoinc") == "enabled";
                ckSuccess.Checked = PrState.Env.GetPreference($"{PID}:success") == "enabled";
            }
            else
            {
                ckAutoinc.Visible = false;
                ckSuccess.Visible = false;
                ckAutoinc.Checked = false;
                ckSuccess.Checked = false;
            }
        }

        /// <summary>
        /// Прошивка завершена
        /// </summary>
        private void Completed(bool Success)
        {
            bStart.Visible = true;
            bCancel.Visible = false;

            if(ckAutoinc.Checked)
            {
                if(ckSuccess.Checked)
                {
                    if (Success) Autoincrement();
                }
                else
                    Autoincrement();
            }
        }

        private void ResetStepsStatus()
        {
            SetStatus(Color.Black, "");
            if (StepsControls != null) 
            {
                for(int i = 0; i < StepsControls.Length; i++)
                    SetStepResult(i, false, null);
            }
        }

        private void StartStep(int Index)
        {
            PrState.RunAction(Index);
            SetStepResult(Index, true, null);
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            bStart.Visible = false; 
            bCancel.Visible = true;
            ApplyOptions();
            SaveOptions();
            ResetStepsStatus();
            StartStep(0);
            SetStatus(Color.Black, "");
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            PrState.Cancelled = true;
        }

        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            PrState.Cancelled = true;
            PrState.Env.Save("config.xml");
        }

        private void mFileOpen_Click(object sender, EventArgs e)
        {

        }
    }
}
