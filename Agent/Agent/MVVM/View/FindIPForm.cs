using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Agent.Model;
using System.Threading;

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
            this.portTextBox.Text = Properties.Settings.Default.Port.ToString();
        }

        private void ipBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length != 0)
            {
                if ((Regex.Match(tb.Text, patternIP)).Length == 0) // Если не соответсвует требованиям IP-адреса
                {
                    tb.Text = oldString;
                }
                else
                {
                    oldString = tb.Text;
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
            ushort port;
            ip.Append(ipBox1.Text).Append('.').Append(ipBox2.Text).Append('.').Append(ipBox3.Text).Append('.').Append(ipBox4.Text);
            if (IPAddress.TryParse(ip.ToString(), out findIP) && ushort.TryParse(portTextBox.Text,out port))
            {
                connectButton.Text = "Подключение";
                connectButton.Enabled = false;
                try
                {
                    agent.ConnectToContractor(findIP,10000,port);
                    agent.SaveAllContractorToDB();
                }
                catch
                {
                    MessageBox.Show("Машина не найдена","Ошибка",MessageBoxButtons.OK);
                }
                finally
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Вами введен не верный IP или порт", "Ошибка ввода", MessageBoxButtons.OK);
            }
        }
    }
}
