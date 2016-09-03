using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Agent
{
    class Log
    {
        private static object sync = new object();
        public static void ShowMessage(string text)
        {
            Log.Write(text);
            Thread th = new Thread(delegate ()
            {
                MessageBox.Show(text);
            });
            th.IsBackground = true;
            th.Start();
        }
        public static void Write(Exception ex)
        {
            string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n",
            DateTime.Now, ex.TargetSite.DeclaringType, ex.TargetSite.Name, ex.Message);
            WriteString(fullText);
        }
        public static void Write(string message)
        {
            string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1}\r\n", DateTime.Now, message);
            WriteString(fullText);
        }
        private static void WriteString(string fullText)
        {
            string pathToLog = Model.AgentSystem.WorkingFolder + "\\Log";
            if (!Directory.Exists(pathToLog))
                Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
            string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
            lock (sync)
            {
                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
            }
        }
    }
}
