using System;
using System.Windows.Forms;
using Agent.Model;
using Agent.View;

namespace Agent
{
    public static class Programm 
    {
        public static void Reset()
        {
            Application.Restart();
            System.Environment.Exit(0);
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AgentSystem ags = new AgentSystem();
            AgentForm agf = new AgentForm(ags);
            foreach(var arg in args)
            {
                if(arg.CompareTo("-autorun")==0)
                    agf.WindowState = FormWindowState.Minimized; // загружать в трей
            }
            Application.Run(agf);
        }
    }
}
