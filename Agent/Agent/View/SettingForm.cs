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
        private bool CheckMask(params string[] parts)
        {
            byte[] bytes = new byte[4]; // формируем массив байт
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = byte.Parse(parts[i]);
                byte temp = bytes[i];
                bool ones = false;
                for(int j=0; j<8; j++) // каждый байт проверяем на соответсвие байту масок
                {
                    if(ones==true && temp % 2 == 0) // если перед единицами есть ноль, значит не маска
                        return false;
                    else if (temp % 2 == 1) // если единица перед нулями, то запоминаем её
                        ones = true;
                    temp = (byte)(temp >> 1); // битовый сдвиг влево
                }
            }
            for (int i = 1; i < 4; i++)
                if ((bytes[i] > bytes[i - 1]) || ((bytes[i] == bytes[i - 1]) && bytes[i] != 255)) 
                    return false;
            return true;
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            StringBuilder mask = new StringBuilder("");
            IPAddress ip;
            int port = 0;
            // Проверка маски
            if (!CheckMask(maskBox1.Text, maskBox2.Text, maskBox3.Text, maskBox4.Text))
            {
                MessageBox.Show("Маска задана не верно. Проверьте настроки");
                return; 
            }
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
