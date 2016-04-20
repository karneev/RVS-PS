using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agent.View
{
    partial class AboutBox : Form
    {
        public AboutBox(Agent.Model.AgentSystem agent)
        {
            InitializeComponent();
            this.Text = String.Format("О программе {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Версия {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            StringBuilder str = new StringBuilder("");
            str.Append("Имя машины: " + Environment.MachineName).Append("\n");
            str.Append("Имя пользователя: " + Environment.UserName).Append("\n");
            str.Append("Текущая платформа(номер версии): " + Environment.OSVersion).Append("\n");
            str.Append("Идентификатор платформы: " + Environment.OSVersion.Platform).Append("\n");
            str.AppendFormat("Время с момента загрузки системы: {0:d}:{1:d2}:{2:d2}", 
                Environment.TickCount / 1000 / 3600, 
                (Environment.TickCount / 1000 % 3600) / 60, 
                Environment.TickCount / 1000 % 60).Append("\n");
            str.Append("Объем физической памяти текущего процесса: " + Environment.WorkingSet / 1024 / 1024).Append("МБ\n");
            str.Append("Объем свободной физической памяти: " + agent.InfoMe.vRam).Append("МБ\n");
            str.Append("Частота процессора: " + agent.InfoMe.vCPU).Append("MHz\n");
            str.Append("Число ядер процессора: " + Environment.ProcessorCount).Append("\n");
            str.Append("Командная строка текущего процесса: " + Environment.CommandLine).Append("\n");
            this.textBoxDescription.Text = str.ToString();
            this.textBoxDescription.Text += AssemblyDescription;
        }

        #region Методы доступа к атрибутам сборки

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!String.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
