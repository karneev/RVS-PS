using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Agent.Model;

namespace Agent.View
{
    public partial class FindIPForm : Form
    {
        AgentSystem agent;
        string oldString;
        static string patternIP = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
        public FindIPForm(AgentSystem agent)
        {
            this.agent = agent;
            InitializeComponent();
        }

        private void ipBox_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.Length != 0)
            {
                if ((Regex.Match(((TextBox)sender).Text, patternIP)).Length == 0) // Если не соответсвует требованиям IP-адреса
                {
                    ((TextBox)sender).Text = oldString;
                }
                else
                {
                    oldString = ((TextBox)sender).Text;
                }
            }
            else
            {
                oldString = "";
            }
        }
        private void pointBox_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = ".";
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            IPAddress findIP;
            StringBuilder ip = new StringBuilder("");
            ip.Append(ipBox1.Text).Append('.').Append(ipBox2.Text).Append('.').Append(ipBox3.Text).Append('.').Append(ipBox4.Text);
            if (IPAddress.TryParse(ip.ToString(), out findIP))
            {
                connectButton.Text = "Подключение";
                connectButton.Enabled = false;
                try
                {
                    agent.ConnectToContractor(findIP);
                    this.Close();
                }
                catch
                {
                    connectButton.Text = "Машина не найдена";
                }
            }
            else
            {
                MessageBox.Show("Вами введен не верный IP", "Ошибка ввода", MessageBoxButtons.OK);
            }
        }
    }
}
