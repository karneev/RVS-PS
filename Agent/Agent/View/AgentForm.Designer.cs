namespace Agent.View
{
    partial class AgentForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgentForm));
            this.startCalcButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.agentNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.fileToolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.startCalculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // startCalcButton
            // 
            this.startCalcButton.Location = new System.Drawing.Point(12, 28);
            this.startCalcButton.Name = "startCalcButton";
            this.startCalcButton.Size = new System.Drawing.Size(137, 23);
            this.startCalcButton.TabIndex = 0;
            this.startCalcButton.Text = "Запустить вычисления";
            this.startCalcButton.UseVisualStyleBackColor = true;
            this.startCalcButton.Click += new System.EventHandler(this.startCalcButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(168, 28);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(75, 23);
            this.settingsButton.TabIndex = 1;
            this.settingsButton.Text = "Настройки";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 63);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(86, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Статус машины";
            // 
            // statusTextBox
            // 
            this.statusTextBox.Enabled = false;
            this.statusTextBox.Location = new System.Drawing.Point(104, 60);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.Size = new System.Drawing.Size(139, 20);
            this.statusTextBox.TabIndex = 3;
            // 
            // agentNotifyIcon
            // 
            this.agentNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.agentNotifyIcon.ContextMenuStrip = this.ContextMenuStrip;
            this.agentNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("agentNotifyIcon.Icon")));
            this.agentNotifyIcon.Text = "Агент";
            this.agentNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.agentNotifyIcon_MouseDoubleClick);
            // 
            // fileToolStripDropDownButton1
            // 
            this.fileToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startCalculateToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileToolStripDropDownButton1.Name = "fileToolStripDropDownButton1";
            this.fileToolStripDropDownButton1.Size = new System.Drawing.Size(49, 22);
            this.fileToolStripDropDownButton1.Text = "Файл";
            // 
            // startCalculateToolStripMenuItem
            // 
            this.startCalculateToolStripMenuItem.Name = "startCalculateToolStripMenuItem";
            this.startCalculateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.startCalculateToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.startCalculateToolStripMenuItem.Text = "Начать вычисления";
            this.startCalculateToolStripMenuItem.Click += new System.EventHandler(this.startCalcButton_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.settingsToolStripMenuItem.Text = "Настройки";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // infoButton
            // 
            this.infoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.infoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(86, 22);
            this.infoButton.Text = "О программе";
            this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripDropDownButton1,
            this.infoButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(251, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AgentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 85);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.startCalcButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AgentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Агент";
            this.Resize += new System.EventHandler(this.AgentForm_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button startCalcButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.NotifyIcon agentNotifyIcon;
        private System.Windows.Forms.ToolStripDropDownButton fileToolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem startCalculateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton infoButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
    }
}

