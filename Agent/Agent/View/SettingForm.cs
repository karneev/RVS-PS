using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Agent.Model;

namespace Agent.View
{
    public partial class SettingForm : Form
    {
        AgentSystem agent;
        string oldString;
        static string patternIP = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
        public SettingForm(ref AgentSystem agent)
        {
            this.agent = agent;
            InitializeComponent();
            int numIP = 0, selectIP=-1;
            string[] mask;
            string ip = Properties.Settings.Default.IP;

            // инициализация системных настроек
            autoRunCheckBox.Checked = Properties.Settings.Default.AutoRun;
            // инициализация сетевых настроек
            IPHostEntry iphostentry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // если адрес ipv4
                {
                    ipComboBox.Items.Add(ipaddress.ToString());
                    if (ipaddress.ToString().CompareTo(ip) == 0)
                        selectIP = numIP;
                    numIP++;
                }
            }
            ipComboBox.SelectedIndex = selectIP;

            mask = Properties.Settings.Default.Mask.Split('.');
            maskBox1.Text = mask[0];
            maskBox2.Text = mask[1];
            maskBox3.Text = mask[2];
            maskBox4.Text = mask[3];
            if (Properties.Settings.Default.Port != 0)
                portBox1.Text = Properties.Settings.Default.Port.ToString();
            else
                portBox1.Text = "";
            oldString = "";
        }

        private void pointBox_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = ".";
        }

        private void ipComboBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                IPAddress.Parse(ipComboBox.Text);
            }
            catch
            {
                ipComboBox.SelectedIndex = -1;
            }
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

        private void portBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text.Length != 0)
            {
                int dec;
                if (Int32.TryParse(tb.Text, out dec))
                {
                    if (dec >= Int32.MinValue && dec <= Int32.MaxValue)
                    {
                        oldString = tb.Text;
                    }
                    else
                    {
                        tb.Text = oldString;
                    }
                }
                else
                {
                    tb.Text = oldString;
                }
            }
            else
            {
                oldString = "";
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            StringBuilder mask = new StringBuilder("");
            IPAddress ip;
            int port = 0;
            // сохранение системных настроек
            Properties.Settings.Default.AutoRun = autoRunCheckBox.Checked;
            agent.UpdateAutoRun();
            // сохранение сетевых настроек
            mask.Append(maskBox1.Text).Append('.').Append(maskBox2.Text).Append('.').Append(maskBox3.Text).Append('.').Append(maskBox4.Text);
            if(!IPAddress.TryParse(ipComboBox.SelectedItem.ToString(), out ip))
                if (!IPAddress.TryParse(ipComboBox.Items[0].ToString(), out ip))
                    ip = IPAddress.Parse("127.0.0.1");
            Properties.Settings.Default.IP = ip.ToString();
            if (!IPAddress.TryParse(mask.ToString(), out ip))
                ip = IPAddress.Parse("255.255.255.0");
            Properties.Settings.Default.Mask = ip.ToString();
            if (!Int32.TryParse(portBox1.Text, out port))
                port = 56001;
            Properties.Settings.Default.Port = port;
            Properties.Settings.Default.Save();
            agent.NetworkSettingsChange();
            this.Close();
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
