namespace Agent.View
{
    partial class SettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.ipPanel2 = new System.Windows.Forms.Panel();
            this.pointBox6 = new System.Windows.Forms.TextBox();
            this.pointBox5 = new System.Windows.Forms.TextBox();
            this.pointBox4 = new System.Windows.Forms.TextBox();
            this.maskBox4 = new System.Windows.Forms.TextBox();
            this.maskBox3 = new System.Windows.Forms.TextBox();
            this.maskBox2 = new System.Windows.Forms.TextBox();
            this.maskBox1 = new System.Windows.Forms.TextBox();
            this.portBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.ipComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.autoRunCheckBox = new System.Windows.Forms.CheckBox();
            this.autoUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.ipPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipPanel2
            // 
            this.ipPanel2.Controls.Add(this.pointBox6);
            this.ipPanel2.Controls.Add(this.pointBox5);
            this.ipPanel2.Controls.Add(this.pointBox4);
            this.ipPanel2.Controls.Add(this.maskBox4);
            this.ipPanel2.Controls.Add(this.maskBox3);
            this.ipPanel2.Controls.Add(this.maskBox2);
            this.ipPanel2.Controls.Add(this.maskBox1);
            this.ipPanel2.Location = new System.Drawing.Point(75, 119);
            this.ipPanel2.Name = "ipPanel2";
            this.ipPanel2.Size = new System.Drawing.Size(136, 19);
            this.ipPanel2.TabIndex = 7;
            // 
            // pointBox6
            // 
            this.pointBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pointBox6.Location = new System.Drawing.Point(98, 3);
            this.pointBox6.Name = "pointBox6";
            this.pointBox6.Size = new System.Drawing.Size(10, 13);
            this.pointBox6.TabIndex = 6;
            this.pointBox6.TabStop = false;
            this.pointBox6.Text = ".";
            this.pointBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pointBox6.TextChanged += new System.EventHandler(this.pointBox_TextChanged);
            // 
            // pointBox5
            // 
            this.pointBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pointBox5.Location = new System.Drawing.Point(63, 3);
            this.pointBox5.Name = "pointBox5";
            this.pointBox5.Size = new System.Drawing.Size(10, 13);
            this.pointBox5.TabIndex = 5;
            this.pointBox5.TabStop = false;
            this.pointBox5.Text = ".";
            this.pointBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pointBox5.TextChanged += new System.EventHandler(this.pointBox_TextChanged);
            // 
            // pointBox4
            // 
            this.pointBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pointBox4.Location = new System.Drawing.Point(28, 3);
            this.pointBox4.Name = "pointBox4";
            this.pointBox4.Size = new System.Drawing.Size(10, 13);
            this.pointBox4.TabIndex = 4;
            this.pointBox4.TabStop = false;
            this.pointBox4.Text = ".";
            this.pointBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pointBox4.TextChanged += new System.EventHandler(this.pointBox_TextChanged);
            // 
            // maskBox4
            // 
            this.maskBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.maskBox4.Location = new System.Drawing.Point(108, 3);
            this.maskBox4.MaxLength = 3;
            this.maskBox4.Name = "maskBox4";
            this.maskBox4.Size = new System.Drawing.Size(25, 13);
            this.maskBox4.TabIndex = 3;
            this.maskBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskBox4.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // maskBox3
            // 
            this.maskBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.maskBox3.Location = new System.Drawing.Point(73, 3);
            this.maskBox3.MaxLength = 3;
            this.maskBox3.Name = "maskBox3";
            this.maskBox3.Size = new System.Drawing.Size(25, 13);
            this.maskBox3.TabIndex = 2;
            this.maskBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskBox3.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // maskBox2
            // 
            this.maskBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.maskBox2.Location = new System.Drawing.Point(38, 3);
            this.maskBox2.MaxLength = 3;
            this.maskBox2.Name = "maskBox2";
            this.maskBox2.Size = new System.Drawing.Size(25, 13);
            this.maskBox2.TabIndex = 1;
            this.maskBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskBox2.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // maskBox1
            // 
            this.maskBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.maskBox1.Location = new System.Drawing.Point(3, 3);
            this.maskBox1.MaxLength = 3;
            this.maskBox1.Name = "maskBox1";
            this.maskBox1.Size = new System.Drawing.Size(25, 13);
            this.maskBox1.TabIndex = 0;
            this.maskBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskBox1.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // portBox1
            // 
            this.portBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.portBox1.Location = new System.Drawing.Point(75, 144);
            this.portBox1.MaxLength = 5;
            this.portBox1.Name = "portBox1";
            this.portBox1.Size = new System.Drawing.Size(38, 13);
            this.portBox1.TabIndex = 8;
            this.portBox1.Text = "56001";
            this.portBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.portBox1.TextChanged += new System.EventHandler(this.portBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(28, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Сетевые настройки";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "IP-адрес";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Маска сети";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Порт";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(73, 163);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 13;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // ipComboBox
            // 
            this.ipComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ipComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ipComboBox.Location = new System.Drawing.Point(75, 94);
            this.ipComboBox.Name = "ipComboBox";
            this.ipComboBox.Size = new System.Drawing.Size(136, 21);
            this.ipComboBox.TabIndex = 14;
            this.ipComboBox.TextChanged += new System.EventHandler(this.ipComboBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(28, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Системные настройки";
            // 
            // autoRunCheckBox
            // 
            this.autoRunCheckBox.AutoSize = true;
            this.autoRunCheckBox.Location = new System.Drawing.Point(22, 29);
            this.autoRunCheckBox.Name = "autoRunCheckBox";
            this.autoRunCheckBox.Size = new System.Drawing.Size(150, 17);
            this.autoRunCheckBox.TabIndex = 17;
            this.autoRunCheckBox.Text = "Автозапуск приложения";
            this.autoRunCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.AutoSize = true;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(22, 46);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(110, 17);
            this.autoUpdateCheckBox.TabIndex = 18;
            this.autoUpdateCheckBox.Text = "Автообновление";
            this.autoUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 190);
            this.Controls.Add(this.autoUpdateCheckBox);
            this.Controls.Add(this.autoRunCheckBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ipComboBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portBox1);
            this.Controls.Add(this.ipPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingForm_FormClosing);
            this.ipPanel2.ResumeLayout(false);
            this.ipPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel ipPanel2;
        private System.Windows.Forms.TextBox pointBox6;
        private System.Windows.Forms.TextBox pointBox5;
        private System.Windows.Forms.TextBox pointBox4;
        private System.Windows.Forms.TextBox maskBox4;
        private System.Windows.Forms.TextBox maskBox3;
        private System.Windows.Forms.TextBox maskBox2;
        private System.Windows.Forms.TextBox maskBox1;
        private System.Windows.Forms.TextBox portBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ComboBox ipComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox autoRunCheckBox;
        private System.Windows.Forms.CheckBox autoUpdateCheckBox;
    }
}