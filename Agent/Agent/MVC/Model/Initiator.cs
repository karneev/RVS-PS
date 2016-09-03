using Agent.Enums;
using Agent.Structs;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Agent.Model
{
    public class Initiator : ILockeded       // класс информации о инициаторе
    {
        AgentSystem agent;
        public event GetMessage NewMessage; // событие получения нового сообщения
        bool started;
        bool locked;                        // заблокирована ли машина на прием пакетов
        TcpListener server;               // сервер (в нашей системе - исполнитель)
        TcpClient client;                 // клииент (в нашей системе - инициатор)
        Thread th;                      // основной поток сервера
        NetworkStream mainStream;       // основной поток
        BinaryFormatter bf = new BinaryFormatter();

        public bool IsStart
        {
            get { return started; }
        }
        public bool Locked
        {
            set { locked = value; }
            get { return locked; }
        }

        public Initiator(AgentSystem agent)
        {
            this.agent = agent;
            started = false;
            Start();   // запускаем сервер 
        }
        void Run()  // работа сервера
        {
            try
            {
                agent.SetFailStatus(false);
                Packet pkt;
                server.Start();                      // запускаем сервер
                client = server.AcceptTcpClient();   // ожидаем инициатора
                mainStream = client.GetStream();     // как инициатор подключился
                while (client.Connected)             // пока клиент подключен - получаем от него сообщения
                {
                    if (locked == false)
                    {
                        pkt = (Packet)bf.Deserialize(mainStream);   // получаем пакет и
                        locked = true;
                        NewMessage(this, pkt.ToString());                 // зажигаем событие "новое сообщение"
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                if (agent.Status.Free == false && agent.Status.Initiator==false && agent.Status.LoadSettings == false)
                {
                    Log.Write("Перезагрузка по причине потери инициатора и мы не свободны");
                    Programm.Reset();
                }
                if(agent.Status.Free==true)
                {
                    agent.SetFailStatus(true);
                }
                if(mainStream!=null)
                    mainStream.Close();
            }
        }
        public void GetExeAndDataFile(bool isExe) // получить файл true-exe, false-data
        {
            if (isExe)
            {
                lock(agent)
                {
                    agent.ExeFile = GetFile();    // принимаем исполняемый файл
                }
            }
            else
            {
                lock (agent)
                {
                    agent.NotDiffDataFile.Add(GetFile()); // принимаем файл данных
                }
            }
        }
        private FileInfo GetFile() // считать файл с сети
        {
            //Programm.ShowMessage("Начали принимать файл с сети"); // отладочный вывод

            FileStream fout;
            HandleFile hf = (HandleFile)bf.Deserialize(mainStream); // считываем загаловок файла
            FileInfo file = new FileInfo(AgentSystem.WorkingFolder + "\\Temp\\"+hf.fileName);
            if (file.Exists) // Создаем файл с заданным именем
                file.Delete();
            fout = file.Create();
            long length = hf.size;
            long position = 0;
            while (position != length)  // принимаем весь файл
            {
                PartFile pf = (PartFile)bf.Deserialize(mainStream);
                fout.Write(pf.part, 0, pf.len);
                position += pf.len;
            }
            fout.Close();
            
            //Programm.ShowMessage("Закончили принимать файл с сети"); // отладочный вывод
            return file;

        }
        public void SendMessage(Packet pkt)     // передать сообщение инициатору
        {
            try
            {
                if (client.Connected == true)
                {
                    bf.Serialize(mainStream, pkt);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
        public void SendInfoMe()    // сообщить информацию о себе
        {
            bf.Serialize(mainStream, agent.InfoMe);       // отправляем инфо о себе
        }
        public void Start() // запуск сервера
        {
            if (started == false)
            {
                if (th == null || th.IsAlive == false)   // если потока нет или он не запущен, то запускаем
                {
                    started = true;
                    server = new TcpListener(IPAddress.Any, Properties.Settings.Default.Port);
                    th = new Thread(Run);
                    th.IsBackground = true;
                    th.Start();
                }
            }
        }
        public void Stop() // останавливаем сервер и ждем завершения потока
        {
            if (started == true)
            {
                started = false;
                if(mainStream!=null)
                    mainStream.Close();
                server.Stop();
                th.Join();
            }
        }
        public void Restart() // останавливаем и запускаем сервер
        {
            Stop();
            Start();
        }
    }
}
