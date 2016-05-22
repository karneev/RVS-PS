using System;
using System.Windows.Forms;
using Agent.Model;
using Agent.View;
using System.Drawing;

namespace Agent
{
    public static class Programm 
    {
        public static void Reset()
        {
            Application.Restart();
            System.Environment.Exit(0);
        }
        private static void NoSleep()
        {
            bool flag = true;
            Timer timer = new Timer();
            timer.Interval = 5;
            timer.Enabled = true;
            timer.Tick += new EventHandler(delegate(object sender, EventArgs e)
            {
                Point p = Cursor.Position;
                int x = p.X;
                int y = p.Y;
                if (flag) Cursor.Position = new Point(x++, y ++);
                else Cursor.Position = new Point(x --, y --);
                flag = !flag;
            });
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            
            AgentSystem ags = new AgentSystem();
            AgentForm agf = new AgentForm(ags);
            agf.WindowState = FormWindowState.Minimized; // загружать в трей
            NoSleep();
            Application.Run(agf);
        }
    }
}
