using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Statistic
{
    public partial class MainForm : Form
    {
        int lastCountClients=-1;
        Server server;
        public MainForm(Server server, Mode mode)
        {
            InitializeComponent();
            this.server = server;
            RunUpdateWindow();
            switch (mode)
            {
                case Mode.Debug:
                    RunDebug();
                    break;
                case Mode.Release:
                    RunRelease();
                    break;
            }
        }
        private void RunUpdateWindow()
        {
            Thread th = new Thread(delegate ()
              {
                  while (true)
                  {
                      informationRichTextBox.Text = server.ToString();
                      Thread.Sleep(1000);
                  }
              });
            th.IsBackground = true;
            th.Start();
        }
        private void RunDebug()
        {
            this.Text += " (Режим отладки)";
            this.BackColor = Color.OrangeRed;
        }
        private void RunRelease()
        {
            CheckIndexHtml();
            Thread th = new Thread(delegate ()
            {
                while (true)
                {
                    if (lastCountClients != server.GetCountClients())
                    {
                        lastCountClients = server.GetCountClients();
                        using (StreamWriter sw = new StreamWriter("text.txt"))
                            sw.WriteLine("Clients in system now: " + lastCountClients);
                    }
                    Thread.Sleep(1000);
                }
            });
            th.IsBackground = true;
            th.Start();
            
        }
        private void CheckIndexHtml()
        {
            Thread th = new Thread(delegate ()
            {
                while (true)
                {
                    FileInfo fi1 = new FileInfo("index.html");
                    FileInfo fi2 = new FileInfo("index.php");
                    using (StreamReader source = new StreamReader(fi1.OpenRead()))
                    {
                        using (StreamWriter dist = new StreamWriter(fi2.OpenWrite()))
                        {
                            string buf = "";
                            while (!buf.Contains("</BODY>"))
                            {
                                dist.WriteLine(buf);
                                buf = source.ReadLine();
                            }
                            dist.WriteLine("<?php echo file_get_contents(\"text.txt\"); ?>");
                            dist.WriteLine(buf);
                            dist.WriteLine(source.ReadLine());
                        }
                    }
                    Thread.Sleep(30000);
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FileInfo fi2 = new FileInfo("index.php");
            if (fi2.Exists)
                fi2.Delete();
        }


        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
            }
        }
    }
}
