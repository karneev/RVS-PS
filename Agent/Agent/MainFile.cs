using System;
using System.Windows.Forms;
using System.Text;
using Agent.Model;
using Agent.View;

namespace Agent
{
    public delegate void RefreshData(); // делегат изменения представления
    public delegate void SetStat(StatusMachine status); // делегат изменение статуса
    public delegate void GetMessage(ILockeded sender, string message); // делегат получение сообщения
    public enum StatusMachine // Статус машины
    {
        Free,
        Wait,
        Initiator,
        Calculate,
        Testing,
        LoadSettings,
        WaitEndCalc
    }
    public enum PacketType // Тип пакета
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
    public struct Packet // Пакет
    {
        public PacketType type;  // Код пакета
        public long id;          // ID источника пакета
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(type).Append(" ").Append(id);
            return str.ToString();
        }
    }
    [Serializable]
    public struct MachineInfo
    {
        [field: NonSerialized] public event SetStat StatusChange;
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
                StatusChange(status);
            }
        }
    }

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
        static void Main()
        {
            AgentSystem ags = new AgentSystem();
            AgentForm agf = new AgentForm(ags);
            Application.Run(agf);
        }
    }
}
