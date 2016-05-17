﻿using System;
using System.Windows.Forms;
using System.Threading;
using Agent.Model;
using Agent.Enums;
using System.IO;

namespace Agent.View
{
    public partial class AgentForm : Form
    {
        AgentSystem agent;
        FormWindowState formerState; // Статус окна (свернуто/развернуто)
        internal AgentForm(AgentSystem agent)
        {
            InitializeComponent();
            this.Visible = true;
            this.agent = agent;
            this.agent.AddListener(setStatus);
            // Запуск теста
            Thread th = new Thread(new ThreadStart(this.agent.TestSystem));
            th.IsBackground = true;
            th.Start();
            // Загрузить настройки. Если их нет - принудительно запросить
            try
            {
                string md = Environment.GetFolderPath(Environment.SpecialFolder.Personal);//путь к Документам
                if (File.Exists(md + "\\Agent\\" + "Settings.conf") == false)
                {
                    if (Properties.Settings.Default.IP.CompareTo("127.0.0.1") == 0)
                    {
                        MessageBox.Show("Настройки не заданы!\nЗадайте настройки перед началом работы!");
                        (new SettingForm(ref this.agent)).ShowDialog();
                    }
                    else
                    {
                        SettingForm.SaveSettingInFile();
                    }
                }
                else
                {
                    SettingForm.LoadSettingFromFile();
                    agent.UpdateAutoRun();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                MessageBox.Show("Настройки не заданы!\nЗадайте настройки перед началом работы!");
                (new SettingForm(ref this.agent)).ShowDialog();
            }
        }
        private void setStatus(StatusMachine status)
        {
            if (this.statusTextBox.InvokeRequired)
            {
                SetStat d = new SetStat(setStatus);
                this.Invoke(d, new object[] { status });
            }
            else
            {
                if (status == StatusMachine.Free) // свободен
                {
                    this.startCalcButton.Enabled = true;
                    this.startCalculateToolStripMenuItem.Enabled = true;
                    this.settingsButton.Enabled = true;
                    this.settingsToolStripMenuItem.Enabled = true;
                    this.statusTextBox.Text = "Свободен";
                    this.agentNotifyIcon.Text = "Свободен";
                }
                else // Занят
                {
                    // Ожидание, инициатор или уже участвует в вычислениях
                    if (status == StatusMachine.Wait || status == StatusMachine.Initiator || status == StatusMachine.Calculate) 
                    {
                        this.settingsButton.Enabled = false;
                        this.settingsToolStripMenuItem.Enabled = false;
                    }
                    else // остальные случаи
                    {
                        this.settingsButton.Enabled = true;
                        this.settingsToolStripMenuItem.Enabled = true;
                    }
                    this.startCalcButton.Enabled = false;
                    this.startCalculateToolStripMenuItem.Enabled = false;
                    this.statusTextBox.Text = "Занят. Код: " + status.ToString();
                    this.agentNotifyIcon.Text = "Занят. Код: " + status.ToString();
                }
            }
        }
        private void AgentForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                formerState = this.WindowState;
            }
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                agentNotifyIcon.Visible = true;
            }
            else
            {
                this.ShowInTaskbar = true;
                agentNotifyIcon.Visible = false;
            }
        }
        private void AgentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            agent.Stop();
            agentNotifyIcon.Visible = false;
        }

        private void agentNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = formerState;
            this.Activate();
        }

        private void startCalcButton_Click(object sender, EventArgs e)
        {
            agent.Status = StatusMachine.Initiator;
            (new CalculateForm(ref agent)).ShowDialog();
            agent.Status = StatusMachine.Free;
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            (new SettingForm(ref agent)).ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            (new AboutBox(agent)).Show();
        }

        private void перезапускПриложенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Programm.Reset();
        }
    }
}
