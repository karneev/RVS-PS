using Agent.Enums;
using Agent.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Agent.Model
{
    public class Contractor : ILockeded   // класс информации о исполнителях
    {
        AgentSystem agent;
        public event GetMessage NewMessage;    // событие получения нового сообщения
        TcpClient client;                      // связь с инициатором
        MachineInfo info;                      //информация о исполнителе
        bool selected;                         // выбран ли этот исполнитель для вычислений
        bool locked;
        bool closing = false;                   // идет ли закрытие соединения инициатором
        Thread th;                             // основной поток исполнителя
        BinaryFormatter bf = new BinaryFormatter();
        NetworkStream mainStream;
        List<FileInfo> dataFile = new List<FileInfo>();

        public bool Locked
        {
            set { locked = value; }
            get { return locked; }
        }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public MachineInfo Info
        {
            get { return info; }
        }

        internal Contractor(AgentSystem agent, TcpClient client)
        {
            this.agent = agent;
            this.client = client;
            mainStream = this.client.GetStream();
            selected = false;
            locked = false;
            bf.Serialize(mainStream, new Packet(){ type=PacketType.Hello, id=agent.InfoMe.id });
            this.info = (MachineInfo)bf.Deserialize(mainStream);
            th = new Thread(RunPacketExchange);
            th.IsBackground = true;
            th.Start();
        }
        void RunPacketExchange()
        {
            try
            {
                Packet pkt;
                while (client.Connected)
                {
                    if (locked == false)
                    {
                        pkt = (Packet)bf.Deserialize(mainStream);          // получаем пакет от исполнителя и
                        locked = true;
                        NewMessage(this, pkt.ToString());                  // зажигаем событие "новое сообщение"
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                if(closing==false)
                    agent.RemoveContractor(this);
                mainStream.Close();
            }
        }
        public bool Connected
        {
            get { return client.Connected; }
        }
        public IPAddress GetIPServer() // получить IP исполнителя
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address;
        }

        public void AddFile(FileInfo file)
        {
            dataFile.Add(file);
        }
        internal void AddFileList(List<FileInfo> fileList)
        {
            foreach(var t in fileList)
                AddFile(t);
        }
        public void SendMessage(Packet pkt) // передать сообщение исполнителю
        {
            bf.Serialize(mainStream, pkt);
        }
        public void SetExeAndDataFile() // отослать exe и data
        {
            lock (agent)
            {
                bf.Serialize(mainStream, new Packet() { type = PacketType.RunFile, id = agent.InfoMe.id }); // сообщаем о том, что будет передача EXE
                sendFile(agent.ExeFile); // передача exe

                foreach (var t in dataFile) // передача dataFiles
                {
                    bf.Serialize(mainStream, new Packet() { type = PacketType.Data, id = agent.InfoMe.id }); // сообщаем о том, что будет передача Data
                    sendFile(t);
                }
            }
            //Programm.ShowMessage("файлы исполнителю отправили");
        }
        private void sendFile(FileInfo file) // отправка файла
        {
            FileStream fin = file.OpenRead(); // открываем файл для передачи
            HandleFile hf = new HandleFile() { fileName = file.Name, size = fin.Length };
            bf.Serialize(mainStream, hf);
            while (fin.Position != fin.Length) // передаем файл
            {
                PartFile pf = new PartFile() { part = new byte[1024] };
                pf.len = fin.Read(pf.part, 0, 1024);
                bf.Serialize(mainStream, pf);
            }
            fin.Close();
        }
        public void Close()
        {
            if (client.Connected)
            {
                closing = true;
                SendMessage(new Packet() { type = PacketType.Free, id = agent.InfoMe.id });
                mainStream.Close();
                client.Close();
                closing = false;
            }
        }
    }
}
