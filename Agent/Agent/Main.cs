using System;
using System.Windows.Forms;
using Agent.Model;
using Agent.View;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Agent
{
    public static class Programm 
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE flags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_CONTINUOUS = 0x80000000
        }


        public static void Reset()
        {
            try
            {
                Application.Restart();
                Environment.Exit(0);
            }
            catch(Exception ex)
            {
                Log.Write("Ошибка при перезапуске");
                Log.Write(ex);
            }
        }
        private static void NoSleep()
        {
            Timer timer = new Timer();
            timer.Interval = 30;
            timer.Enabled = true;
            timer.Tick += new EventHandler(delegate(object sender, EventArgs e)
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS); //  | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
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
