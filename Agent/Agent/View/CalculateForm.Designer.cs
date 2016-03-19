namespace Agent.View
{
    partial class CalculateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.machineSelectPanel = new System.Windows.Forms.Panel();
            this.findIP = new System.Windows.Forms.Button();
            this.checkLabel = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.unchekedAllButton = new System.Windows.Forms.Button();
            this.acceptButton = new System.Windows.Forms.Button();
            this.checkedMechineListBox = new System.Windows.Forms.CheckedListBox();
            this.machineInfoPanel = new System.Windows.Forms.Panel();
            this.unselectAllButton = new System.Windows.Forms.Button();
            this.unselectButton = new System.Windows.Forms.Button();
            this.selectedMachineListBox = new System.Windows.Forms.CheckedListBox();
            this.generalSelectLabel = new System.Windows.Forms.Label();
            this.cpuTextBox = new System.Windows.Forms.TextBox();
            this.cpuLabel = new System.Windows.Forms.Label();
            this.ramTextBox = new System.Windows.Forms.TextBox();
            this.ramLabel = new System.Windows.Forms.Label();
            this.startCalculateButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.filePanel = new System.Windows.Forms.Panel();
            this.dataFileList = new System.Windows.Forms.ListBox();
            this.dataFileLabel = new System.Windows.Forms.Label();
            this.selectRunFileButton = new System.Windows.Forms.Button();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.runFileLabel = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.startInInitiator = new System.Windows.Forms.CheckBox();
            this.machineSelectPanel.SuspendLayout();
            this.machineInfoPanel.SuspendLayout();
            this.filePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // machineSelectPanel
            // 
            this.machineSelectPanel.Controls.Add(this.findIP);
            this.machineSelectPanel.Controls.Add(this.checkLabel);
            this.machineSelectPanel.Controls.Add(this.refreshButton);
            this.machineSelectPanel.Controls.Add(this.unchekedAllButton);
            this.machineSelectPanel.Controls.Add(this.acceptButton);
            this.machineSelectPanel.Controls.Add(this.checkedMechineListBox);
            this.machineSelectPanel.Location = new System.Drawing.Point(12, 12);
            this.machineSelectPanel.Name = "machineSelectPanel";
            this.machineSelectPanel.Size = new System.Drawing.Size(195, 313);
            this.machineSelectPanel.TabIndex = 0;
            // 
            // findIP
            // 
            this.findIP.Location = new System.Drawing.Point(3, 225);
            this.findIP.Name = "findIP";
            this.findIP.Size = new System.Drawing.Size(188, 23);
            this.findIP.TabIndex = 6;
            this.findIP.Text = "Найти по IP";
            this.findIP.UseVisualStyleBackColor = true;
            this.findIP.Click += new System.EventHandler(this.findIP_Click);
            // 
            // checkLabel
            // 
            this.checkLabel.AutoSize = true;
            this.checkLabel.Location = new System.Drawing.Point(43, 3);
            this.checkLabel.Name = "checkLabel";
            this.checkLabel.Size = new System.Drawing.Size(111, 13);
            this.checkLabel.TabIndex = 4;
            this.checkLabel.Text = "Выберите из списка";
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(4, 283);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(188, 23);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Обновить список";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // unchekedAllButton
            // 
            this.unchekedAllButton.Location = new System.Drawing.Point(103, 254);
            this.unchekedAllButton.Name = "unchekedAllButton";
            this.unchekedAllButton.Size = new System.Drawing.Size(89, 23);
            this.unchekedAllButton.TabIndex = 2;
            this.unchekedAllButton.Text = "Снять всё";
            this.unchekedAllButton.UseVisualStyleBackColor = true;
            this.unchekedAllButton.Click += new System.EventHandler(this.unchekedAllButton_Click);
            // 
            // acceptButton
            // 
            this.acceptButton.Location = new System.Drawing.Point(4, 254);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(89, 23);
            this.acceptButton.TabIndex = 1;
            this.acceptButton.Text = "Принять";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // checkedMechineListBox
            // 
            this.checkedMechineListBox.BackColor = System.Drawing.SystemColors.Menu;
            this.checkedMechineListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkedMechineListBox.FormattingEnabled = true;
            this.checkedMechineListBox.Location = new System.Drawing.Point(4, 17);
            this.checkedMechineListBox.Name = "checkedMechineListBox";
            this.checkedMechineListBox.Size = new System.Drawing.Size(188, 202);
            this.checkedMechineListBox.TabIndex = 0;
            // 
            // machineInfoPanel
            // 
            this.machineInfoPanel.Controls.Add(this.unselectAllButton);
            this.machineInfoPanel.Controls.Add(this.unselectButton);
            this.machineInfoPanel.Controls.Add(this.selectedMachineListBox);
            this.machineInfoPanel.Controls.Add(this.generalSelectLabel);
            this.machineInfoPanel.Controls.Add(this.cpuTextBox);
            this.machineInfoPanel.Controls.Add(this.cpuLabel);
            this.machineInfoPanel.Controls.Add(this.ramTextBox);
            this.machineInfoPanel.Controls.Add(this.ramLabel);
            this.machineInfoPanel.Location = new System.Drawing.Point(214, 12);
            this.machineInfoPanel.Name = "machineInfoPanel";
            this.machineInfoPanel.Size = new System.Drawing.Size(232, 313);
            this.machineInfoPanel.TabIndex = 1;
            // 
            // unselectAllButton
            // 
            this.unselectAllButton.Location = new System.Drawing.Point(121, 283);
            this.unselectAllButton.Name = "unselectAllButton";
            this.unselectAllButton.Size = new System.Drawing.Size(90, 23);
            this.unselectAllButton.TabIndex = 11;
            this.unselectAllButton.Text = "Убрать все";
            this.unselectAllButton.UseVisualStyleBackColor = true;
            this.unselectAllButton.Click += new System.EventHandler(this.unselectAllButton_Click);
            // 
            // unselectButton
            // 
            this.unselectButton.Location = new System.Drawing.Point(25, 283);
            this.unselectButton.Name = "unselectButton";
            this.unselectButton.Size = new System.Drawing.Size(87, 23);
            this.unselectButton.TabIndex = 10;
            this.unselectButton.Text = "Убрать";
            this.unselectButton.UseVisualStyleBackColor = true;
            this.unselectButton.Click += new System.EventHandler(this.unselectButton_Click);
            // 
            // selectedMachineListBox
            // 
            this.selectedMachineListBox.BackColor = System.Drawing.SystemColors.Menu;
            this.selectedMachineListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.selectedMachineListBox.FormattingEnabled = true;
            this.selectedMachineListBox.Location = new System.Drawing.Point(25, 39);
            this.selectedMachineListBox.Name = "selectedMachineListBox";
            this.selectedMachineListBox.Size = new System.Drawing.Size(188, 238);
            this.selectedMachineListBox.TabIndex = 5;
            // 
            // generalSelectLabel
            // 
            this.generalSelectLabel.AutoSize = true;
            this.generalSelectLabel.Location = new System.Drawing.Point(22, 4);
            this.generalSelectLabel.Name = "generalSelectLabel";
            this.generalSelectLabel.Size = new System.Drawing.Size(189, 13);
            this.generalSelectLabel.TabIndex = 9;
            this.generalSelectLabel.Text = "Мощность всех выборанных машин";
            // 
            // cpuTextBox
            // 
            this.cpuTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cpuTextBox.Enabled = false;
            this.cpuTextBox.Location = new System.Drawing.Point(155, 20);
            this.cpuTextBox.Name = "cpuTextBox";
            this.cpuTextBox.Size = new System.Drawing.Size(56, 13);
            this.cpuTextBox.TabIndex = 8;
            this.cpuTextBox.Text = "0";
            // 
            // cpuLabel
            // 
            this.cpuLabel.AutoSize = true;
            this.cpuLabel.Location = new System.Drawing.Point(118, 20);
            this.cpuLabel.Name = "cpuLabel";
            this.cpuLabel.Size = new System.Drawing.Size(29, 13);
            this.cpuLabel.TabIndex = 7;
            this.cpuLabel.Text = "CPU";
            // 
            // ramTextBox
            // 
            this.ramTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ramTextBox.Enabled = false;
            this.ramTextBox.Location = new System.Drawing.Point(56, 20);
            this.ramTextBox.Name = "ramTextBox";
            this.ramTextBox.Size = new System.Drawing.Size(56, 13);
            this.ramTextBox.TabIndex = 4;
            this.ramTextBox.Text = "0";
            // 
            // ramLabel
            // 
            this.ramLabel.AutoSize = true;
            this.ramLabel.Location = new System.Drawing.Point(19, 20);
            this.ramLabel.Name = "ramLabel";
            this.ramLabel.Size = new System.Drawing.Size(31, 13);
            this.ramLabel.TabIndex = 2;
            this.ramLabel.Text = "RAM";
            // 
            // startCalculateButton
            // 
            this.startCalculateButton.Location = new System.Drawing.Point(15, 220);
            this.startCalculateButton.Name = "startCalculateButton";
            this.startCalculateButton.Size = new System.Drawing.Size(189, 23);
            this.startCalculateButton.TabIndex = 10;
            this.startCalculateButton.Text = "Начать";
            this.startCalculateButton.UseVisualStyleBackColor = true;
            this.startCalculateButton.Click += new System.EventHandler(this.startCalculateButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(15, 270);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(189, 23);
            this.exitButton.TabIndex = 12;
            this.exitButton.Text = "Выход";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // filePanel
            // 
            this.filePanel.Controls.Add(this.startInInitiator);
            this.filePanel.Controls.Add(this.dataFileList);
            this.filePanel.Controls.Add(this.exitButton);
            this.filePanel.Controls.Add(this.dataFileLabel);
            this.filePanel.Controls.Add(this.selectRunFileButton);
            this.filePanel.Controls.Add(this.fileNameLabel);
            this.filePanel.Controls.Add(this.runFileLabel);
            this.filePanel.Controls.Add(this.startCalculateButton);
            this.filePanel.Location = new System.Drawing.Point(454, 12);
            this.filePanel.Name = "filePanel";
            this.filePanel.Size = new System.Drawing.Size(233, 312);
            this.filePanel.TabIndex = 2;
            // 
            // dataFileList
            // 
            this.dataFileList.FormattingEnabled = true;
            this.dataFileList.Items.AddRange(new object[] {
            "Добавить файл"});
            this.dataFileList.Location = new System.Drawing.Point(6, 93);
            this.dataFileList.Name = "dataFileList";
            this.dataFileList.Size = new System.Drawing.Size(223, 121);
            this.dataFileList.TabIndex = 13;
            this.dataFileList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataFileList_KeyDown);
            this.dataFileList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataFileList_MouseDoubleClick);
            // 
            // dataFileLabel
            // 
            this.dataFileLabel.AutoSize = true;
            this.dataFileLabel.Location = new System.Drawing.Point(3, 77);
            this.dataFileLabel.Name = "dataFileLabel";
            this.dataFileLabel.Size = new System.Drawing.Size(87, 13);
            this.dataFileLabel.TabIndex = 3;
            this.dataFileLabel.Text = "Файлы данных:";
            // 
            // selectRunFileButton
            // 
            this.selectRunFileButton.Location = new System.Drawing.Point(13, 41);
            this.selectRunFileButton.Name = "selectRunFileButton";
            this.selectRunFileButton.Size = new System.Drawing.Size(191, 23);
            this.selectRunFileButton.TabIndex = 2;
            this.selectRunFileButton.Text = "Установить файл";
            this.selectRunFileButton.UseVisualStyleBackColor = true;
            this.selectRunFileButton.Click += new System.EventHandler(this.selectRunFileButton_Click);
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(10, 21);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(60, 13);
            this.fileNameLabel.TabIndex = 1;
            this.fileNameLabel.Text = "не выбран";
            // 
            // runFileLabel
            // 
            this.runFileLabel.AutoSize = true;
            this.runFileLabel.Location = new System.Drawing.Point(3, 3);
            this.runFileLabel.Name = "runFileLabel";
            this.runFileLabel.Size = new System.Drawing.Size(108, 13);
            this.runFileLabel.TabIndex = 0;
            this.runFileLabel.Text = "Исполняемый файл";
            // 
            // startInInitiator
            // 
            this.startInInitiator.AutoSize = true;
            this.startInInitiator.Location = new System.Drawing.Point(30, 247);
            this.startInInitiator.Name = "startInInitiator";
            this.startInInitiator.Size = new System.Drawing.Size(155, 17);
            this.startInInitiator.TabIndex = 14;
            this.startInInitiator.Text = "Запустить на инициаторе";
            this.startInInitiator.UseVisualStyleBackColor = true;
            // 
            // CalculateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 330);
            this.Controls.Add(this.filePanel);
            this.Controls.Add(this.machineInfoPanel);
            this.Controls.Add(this.machineSelectPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CalculateForm";
            this.Text = "Запуск вычислений";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalculateForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CalculateForm_FormClosed);
            this.machineSelectPanel.ResumeLayout(false);
            this.machineSelectPanel.PerformLayout();
            this.machineInfoPanel.ResumeLayout(false);
            this.machineInfoPanel.PerformLayout();
            this.filePanel.ResumeLayout(false);
            this.filePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel machineSelectPanel;
        private System.Windows.Forms.CheckedListBox checkedMechineListBox;
        private System.Windows.Forms.Label checkLabel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button unchekedAllButton;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Panel machineInfoPanel;
        private System.Windows.Forms.TextBox cpuTextBox;
        private System.Windows.Forms.Label cpuLabel;
        private System.Windows.Forms.TextBox ramTextBox;
        private System.Windows.Forms.Label ramLabel;
        private System.Windows.Forms.Label generalSelectLabel;
        private System.Windows.Forms.Button startCalculateButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Panel filePanel;
        private System.Windows.Forms.ListBox dataFileList;
        private System.Windows.Forms.Label dataFileLabel;
        private System.Windows.Forms.Button selectRunFileButton;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Label runFileLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckedListBox selectedMachineListBox;
        private System.Windows.Forms.Button unselectAllButton;
        private System.Windows.Forms.Button unselectButton;
        private System.Windows.Forms.Button findIP;
        private System.Windows.Forms.CheckBox startInInitiator;
    }
}