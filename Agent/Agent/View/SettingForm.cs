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
            byte[] mask;
            int numIP = 0, selectIP=-1;
            
            IPHostEntry iphostentry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // если адрес ipv4
                {
                    ipComboBox.Items.Add(ipaddress.ToString());
                    if (ipaddress.ToString().CompareTo(this.agent.IP.ToString()) == 0)
                        selectIP = numIP;
                    numIP++;
                }
            }
            ipComboBox.SelectedIndex = selectIP;

            mask = this.agent.Mask.GetAddressBytes();
            maskBox1.Text = mask[0].ToString();
            maskBox2.Text = mask[1].ToString();
            maskBox3.Text = mask[2].ToString();
            maskBox4.Text = mask[3].ToString();
            portBox1.Text = this.agent.Port.ToString();
            oldString = "";
            ipPanel2.Enabled = false; // на время не рабочих масок. Маска всегда 255.255.255.0
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

        private void portBox_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.Length != 0)
            {
                int dec;
                if (Int32.TryParse(((TextBox)sender).Text, out dec))
                {
                    if (dec >= Int32.MinValue && dec <= Int32.MaxValue)
                    {
                        oldString = ((TextBox)sender).Text;
                    }
                    else
                    {
                        ((TextBox)sender).Text = oldString;
                    }
                }
                else
                {
                    ((TextBox)sender).Text = oldString;
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
            int port =0;
            mask.Append(maskBox1.Text).Append('.').Append(maskBox2.Text).Append('.').Append(maskBox3.Text).Append('.').Append(maskBox4.Text);

            if(!IPAddress.TryParse(ipComboBox.SelectedItem.ToString(), out ip))
                if (!IPAddress.TryParse(ipComboBox.Items[0].ToString(), out ip))
                    ip = IPAddress.Parse("127.0.0.1");
            this.agent.IP = ip;
            if (!IPAddress.TryParse(mask.ToString(), out ip))
                ip = IPAddress.Parse("255.255.255.0");
            this.agent.Mask = ip;
            if (!Int32.TryParse(portBox1.Text, out port))
                port = 56001;
            this.agent.Port = port;
            this.Close();            
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter sw = new StreamWriter("set.cfg");
            sw.WriteLine(this.agent.IP.ToString());
            sw.WriteLine(this.agent.Mask.ToString());
            sw.WriteLine(this.agent.Port.ToString());
            sw.Close();
        }
    }
}
