using System;
using System.Windows.Forms;
using System.Net;
using System.Text;
using System.Threading;
using Agent.Model;
using Agent.View;

namespace Agent
{

    public delegate void refreshData(); // делегат изменения представления
    public delegate void setStat(StatusMachine status); // делегат изменение статуса
    public delegate void getMessage(ILockeded sender, string message); // делегат получение сообщения
    public enum StatusMachine
    {
        Free,
        Wait,
        Initiator,
        Calculate,
        Testing,
        LoadSettings,
        WaitEndCalc
    }
    public enum PacketType
    {
        Hello,
        RunFile,
        Data,
        FinishCalc,
        StartCalc,
        StopCalc,
        Free,
        NotDeleteFiles,
        Empty
    }

    [Serializable]
    public struct HandleFile // заголовок файла
    {
        public string fileName; // имя файла
        public long size;       // его размерность
    }
    [Serializable]
    public struct PartFile // часть файла
    {
        public int len; // длина части файла
        public byte[] part; // часть файла
    }
    [Serializable]
    public struct Packet
    {
        public PacketType type;  // Код пакета
        public long id;          // ID источника пакета
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("pkt ").Append(type).Append(" ").Append(id);
            return str.ToString();
        }
        public bool Parse(string str) // придумать, как уйти от констант
        {
            try
            {
                string[] words = str.Split(' ');
                switch(words[1])
                {
                    case "Hello":
                        type = PacketType.Hello;
                        break;
                    case "Run":
                        type = PacketType.RunFile;
                        break;
                    case "Data":
                        type = PacketType.Data;
                        break;
                    case "FinishCalc":
                        type = PacketType.FinishCalc;
                        break;
                    case "StartCalc":
                        type = PacketType.StartCalc;
                        break;
                    case "StopCalc":
                        type = PacketType.StopCalc;
                        break;
                    case "Free":
                        type = PacketType.Free;
                        break;
                    default:
                        type = PacketType.Empty;
                        break;
                }
                id = Convert.ToInt64(words[2]);
                return true;
            }
            catch (Exception ex)
            {
                //Programm.ShowMessage(e.Message + " in Parse");
            }
            return false;
        }
    }
    [Serializable]
    public struct MachineInfo
    {
        [field: NonSerialized] public event setStat statusChange;
        public long id;        // ID машины
        public int vRam;       // Объем RAM
        public int vCPU;       // Частота CPU
        private StatusMachine status;    // Статус машины
        public StatusMachine Status
        {
            get
            {
                return this.status;
            }
            set
            {
                status = value;
                statusChange(status);
            }
        }
    }
    public struct SettingSystem
    {
        public IPAddress ip;    // IP
        public IPAddress mask;  // маска
        public int port;        // порт
        public SettingSystem(string ip, string mask, int port)
        {
            try
            {
                this.ip = IPAddress.Parse(ip);
                this.mask = IPAddress.Parse(mask);
            }
            catch
            {
                this.ip = IPAddress.Parse("127.0.0.1");
                this.mask = IPAddress.Parse("255.255.255.0");
            }
            this.port = port;
        }
    }

    public static class Programm 
    {
        static AgentSystem ags;
        static AgentForm agf;
        public static void ShowMessage(String text)
        {
            Thread th = new Thread(delegate ()
              {
                  MessageBox.Show(text);
              });
            th.IsBackground = true;
            th.Start();
        }
        static void Run()
        {
            ags = new AgentSystem();
            agf = new AgentForm(ags);
            Application.Run(agf);
        }
        public static void Reset()
        {
            Application.Restart();
            System.Environment.Exit(0);
        }
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Run();
        }
    }
}
