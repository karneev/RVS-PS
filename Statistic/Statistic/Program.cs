using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Statistic
{
    public enum Mode
    {
        Debug = 0,
        Release = 1
    }
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(@"D:\Apache24\htdocs");
            Server server = new Server();
            if (args.Contains("Debug"))
                Application.Run(new MainForm(server, Mode.Debug));
            else
                Application.Run(new MainForm(server, Mode.Release));
        }
    }
}
