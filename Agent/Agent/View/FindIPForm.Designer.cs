namespace Agent.View
{
    partial class FindIPForm
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
            this.ipPanel2 = new System.Windows.Forms.Panel();
            this.pointBox6 = new System.Windows.Forms.TextBox();
            this.pointBox5 = new System.Windows.Forms.TextBox();
            this.pointBox4 = new System.Windows.Forms.TextBox();
            this.ipBox4 = new System.Windows.Forms.TextBox();
            this.ipBox3 = new System.Windows.Forms.TextBox();
            this.ipBox2 = new System.Windows.Forms.TextBox();
            this.ipBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.ipPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipPanel2
            // 
            this.ipPanel2.Controls.Add(this.pointBox6);
            this.ipPanel2.Controls.Add(this.pointBox5);
            this.ipPanel2.Controls.Add(this.pointBox4);
            this.ipPanel2.Controls.Add(this.ipBox4);
            this.ipPanel2.Controls.Add(this.ipBox3);
            this.ipPanel2.Controls.Add(this.ipBox2);
            this.ipPanel2.Controls.Add(this.ipBox1);
            this.ipPanel2.Location = new System.Drawing.Point(63, 6);
            this.ipPanel2.Name = "ipPanel2";
            this.ipPanel2.Size = new System.Drawing.Size(136, 19);
            this.ipPanel2.TabIndex = 8;
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
            // ipBox4
            // 
            this.ipBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ipBox4.Location = new System.Drawing.Point(108, 3);
            this.ipBox4.MaxLength = 3;
            this.ipBox4.Name = "ipBox4";
            this.ipBox4.Size = new System.Drawing.Size(25, 13);
            this.ipBox4.TabIndex = 3;
            this.ipBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ipBox4.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // ipBox3
            // 
            this.ipBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ipBox3.Location = new System.Drawing.Point(73, 3);
            this.ipBox3.MaxLength = 3;
            this.ipBox3.Name = "ipBox3";
            this.ipBox3.Size = new System.Drawing.Size(25, 13);
            this.ipBox3.TabIndex = 2;
            this.ipBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ipBox3.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // ipBox2
            // 
            this.ipBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ipBox2.Location = new System.Drawing.Point(38, 3);
            this.ipBox2.MaxLength = 3;
            this.ipBox2.Name = "ipBox2";
            this.ipBox2.Size = new System.Drawing.Size(25, 13);
            this.ipBox2.TabIndex = 1;
            this.ipBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ipBox2.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // ipBox1
            // 
            this.ipBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ipBox1.Location = new System.Drawing.Point(3, 3);
            this.ipBox1.MaxLength = 3;
            this.ipBox1.Name = "ipBox1";
            this.ipBox1.Size = new System.Drawing.Size(25, 13);
            this.ipBox1.TabIndex = 0;
            this.ipBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ipBox1.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "IP-адрес";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(50, 31);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(111, 23);
            this.connectButton.TabIndex = 12;
            this.connectButton.Text = "Подключиться";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // FindIPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(209, 58);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ipPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FindIPForm";
            this.Text = "Ручной поиск";
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
        private System.Windows.Forms.TextBox ipBox4;
        private System.Windows.Forms.TextBox ipBox3;
        private System.Windows.Forms.TextBox ipBox2;
        private System.Windows.Forms.TextBox ipBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button connectButton;
    }
}