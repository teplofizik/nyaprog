namespace NyaProg
{
    partial class fMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.cbProject = new System.Windows.Forms.ComboBox();
            this.gbSteps = new System.Windows.Forms.GroupBox();
            this.tTabs = new System.Windows.Forms.TabControl();
            this.tScripts = new System.Windows.Forms.TabPage();
            this.gbScripts = new System.Windows.Forms.GroupBox();
            this.tOptions = new System.Windows.Forms.TabPage();
            this.lResult = new System.Windows.Forms.Label();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bStart = new System.Windows.Forms.Button();
            this.bBack = new System.Windows.Forms.Button();
            this.bOpen = new System.Windows.Forms.Button();
            this.mMenu = new System.Windows.Forms.MenuStrip();
            this.mFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ckSuccess = new System.Windows.Forms.CheckBox();
            this.ckAutoinc = new System.Windows.Forms.CheckBox();
            this.tTabs.SuspendLayout();
            this.tScripts.SuspendLayout();
            this.tOptions.SuspendLayout();
            this.gbOptions.SuspendLayout();
            this.mMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbProject
            // 
            this.cbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProject.FormattingEnabled = true;
            this.cbProject.Location = new System.Drawing.Point(12, 37);
            this.cbProject.Name = "cbProject";
            this.cbProject.Size = new System.Drawing.Size(723, 21);
            this.cbProject.TabIndex = 0;
            // 
            // gbSteps
            // 
            this.gbSteps.Location = new System.Drawing.Point(616, 64);
            this.gbSteps.Name = "gbSteps";
            this.gbSteps.Size = new System.Drawing.Size(200, 513);
            this.gbSteps.TabIndex = 1;
            this.gbSteps.TabStop = false;
            this.gbSteps.Text = "Этапы";
            // 
            // tTabs
            // 
            this.tTabs.Controls.Add(this.tScripts);
            this.tTabs.Controls.Add(this.tOptions);
            this.tTabs.Location = new System.Drawing.Point(12, 64);
            this.tTabs.Name = "tTabs";
            this.tTabs.SelectedIndex = 0;
            this.tTabs.Size = new System.Drawing.Size(596, 517);
            this.tTabs.TabIndex = 2;
            // 
            // tScripts
            // 
            this.tScripts.Controls.Add(this.gbScripts);
            this.tScripts.Location = new System.Drawing.Point(4, 22);
            this.tScripts.Name = "tScripts";
            this.tScripts.Padding = new System.Windows.Forms.Padding(3);
            this.tScripts.Size = new System.Drawing.Size(588, 491);
            this.tScripts.TabIndex = 0;
            this.tScripts.Text = "Выбор скрипта";
            this.tScripts.UseVisualStyleBackColor = true;
            // 
            // gbScripts
            // 
            this.gbScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbScripts.Location = new System.Drawing.Point(3, 3);
            this.gbScripts.Name = "gbScripts";
            this.gbScripts.Size = new System.Drawing.Size(579, 407);
            this.gbScripts.TabIndex = 0;
            this.gbScripts.TabStop = false;
            this.gbScripts.Text = "Доступные варианты действий";
            // 
            // tOptions
            // 
            this.tOptions.Controls.Add(this.lResult);
            this.tOptions.Controls.Add(this.gbOptions);
            this.tOptions.Location = new System.Drawing.Point(4, 22);
            this.tOptions.Name = "tOptions";
            this.tOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tOptions.Size = new System.Drawing.Size(588, 491);
            this.tOptions.TabIndex = 1;
            this.tOptions.Text = "Скрипт";
            this.tOptions.UseVisualStyleBackColor = true;
            // 
            // lResult
            // 
            this.lResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lResult.Location = new System.Drawing.Point(6, 413);
            this.lResult.Name = "lResult";
            this.lResult.Size = new System.Drawing.Size(576, 75);
            this.lResult.TabIndex = 3;
            // 
            // gbOptions
            // 
            this.gbOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOptions.Controls.Add(this.ckAutoinc);
            this.gbOptions.Controls.Add(this.ckSuccess);
            this.gbOptions.Controls.Add(this.bCancel);
            this.gbOptions.Controls.Add(this.bStart);
            this.gbOptions.Controls.Add(this.bBack);
            this.gbOptions.Location = new System.Drawing.Point(3, 3);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(579, 407);
            this.gbOptions.TabIndex = 2;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Скрипт";
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.bCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bCancel.Location = new System.Drawing.Point(373, 361);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(200, 40);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "Отмена";
            this.bCancel.UseVisualStyleBackColor = false;
            this.bCancel.Visible = false;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bStart
            // 
            this.bStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.bStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bStart.Location = new System.Drawing.Point(373, 361);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(200, 40);
            this.bStart.TabIndex = 2;
            this.bStart.Text = "Старт";
            this.bStart.UseVisualStyleBackColor = false;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // bBack
            // 
            this.bBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bBack.Location = new System.Drawing.Point(6, 361);
            this.bBack.Name = "bBack";
            this.bBack.Size = new System.Drawing.Size(126, 40);
            this.bBack.TabIndex = 1;
            this.bBack.Text = "Назад";
            this.bBack.UseVisualStyleBackColor = true;
            this.bBack.Click += new System.EventHandler(this.bBack_Click);
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(741, 36);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(75, 22);
            this.bOpen.TabIndex = 3;
            this.bOpen.Text = "Открыть";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // mMenu
            // 
            this.mMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFile});
            this.mMenu.Location = new System.Drawing.Point(0, 0);
            this.mMenu.Name = "mMenu";
            this.mMenu.Size = new System.Drawing.Size(826, 24);
            this.mMenu.TabIndex = 4;
            this.mMenu.Text = "menuStrip1";
            // 
            // mFile
            // 
            this.mFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileOpen});
            this.mFile.Name = "mFile";
            this.mFile.Size = new System.Drawing.Size(48, 20);
            this.mFile.Text = "Файл";
            // 
            // mFileOpen
            // 
            this.mFileOpen.Name = "mFileOpen";
            this.mFileOpen.Size = new System.Drawing.Size(130, 22);
            this.mFileOpen.Text = "Открыть...";
            // 
            // ckSuccess
            // 
            this.ckSuccess.AutoSize = true;
            this.ckSuccess.Checked = true;
            this.ckSuccess.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckSuccess.Location = new System.Drawing.Point(375, 338);
            this.ckSuccess.Name = "ckSuccess";
            this.ckSuccess.Size = new System.Drawing.Size(204, 17);
            this.ckSuccess.TabIndex = 4;
            this.ckSuccess.Text = "Только по успешному завершению";
            this.ckSuccess.UseVisualStyleBackColor = true;
            // 
            // ckAutoinc
            // 
            this.ckAutoinc.AutoSize = true;
            this.ckAutoinc.Location = new System.Drawing.Point(375, 315);
            this.ckAutoinc.Name = "ckAutoinc";
            this.ckAutoinc.Size = new System.Drawing.Size(146, 17);
            this.ckAutoinc.TabIndex = 5;
            this.ckAutoinc.Text = "Автоинкремент номера";
            this.ckAutoinc.UseVisualStyleBackColor = true;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 593);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.tTabs);
            this.Controls.Add(this.gbSteps);
            this.Controls.Add(this.cbProject);
            this.Controls.Add(this.mMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mMenu;
            this.Name = "fMain";
            this.Text = "NyaProg";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMain_FormClosing);
            this.Load += new System.EventHandler(this.fMain_Load);
            this.tTabs.ResumeLayout(false);
            this.tScripts.ResumeLayout(false);
            this.tOptions.ResumeLayout(false);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            this.mMenu.ResumeLayout(false);
            this.mMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbProject;
        private System.Windows.Forms.GroupBox gbSteps;
        private System.Windows.Forms.TabControl tTabs;
        private System.Windows.Forms.TabPage tScripts;
        private System.Windows.Forms.TabPage tOptions;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.Button bBack;
        private System.Windows.Forms.GroupBox gbScripts;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.MenuStrip mMenu;
        private System.Windows.Forms.ToolStripMenuItem mFile;
        private System.Windows.Forms.ToolStripMenuItem mFileOpen;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Label lResult;
        private System.Windows.Forms.CheckBox ckSuccess;
        private System.Windows.Forms.CheckBox ckAutoinc;
    }
}

