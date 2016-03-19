﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agent.Model
{
    public class AgentSystem
    {
        public event refreshData refreshView;

        private bool isInitiator; // Является ли инициатором
        private bool isCalculate; // начались ли вычисления

        int countFinished;
        private FileInfo exeFile;
        private List<FileInfo> dataFile;
        private List<Contractor> allContractor;  // список всех клиентов (для работы в качестве инициатора)
        private Initiator initiator;            // информация о инициаторе

        private SettingSystem settingSystem;// Сетевые настройки системы
        private MachineInfo infoMe;         // Информация о себе

        public bool IsInitiator
        {
            get { return isInitiator; }
            set { isInitiator = value; }
        }
        public bool IsCalculate
        {
            get { return isCalculate; }
        }
        public StatusMachine Status
        {
            get { return infoMe.Status; }
            set
            {
                infoMe.Status = value;
                if (initiator != null)
                {
                    if (infoMe.Status == StatusMachine.Initiator)
                    {
                        isInitiator = true;
                        initiator.Stop();
                    }
                    else
                    {
                        if (isInitiator == true)
                            isInitiator = true;
                        initiator.Start();
                        isInitiator = false;
                    }
                }
            }
        }
        public IPAddress IP
        {
            get { return settingSystem.ip; }
            set { settingSystem.ip = value; }
        }
        public IPAddress Mask
        {
            get { return settingSystem.mask; }
            set { settingSystem.mask = value; }
        }
        public int Port
        {
            get { return settingSystem.port; }
            set { settingSystem.port = value; }
        }
        public MachineInfo InfoMe
        {
            get { return infoMe; }
        }
        public FileInfo ExeFile
        {
            get
            {
                return exeFile;
            }
            set
            {
                exeFile = value;
                if (isInitiator == true)
                    refreshView();
            }
        }
        public List<FileInfo> DataFile
        {
            get
            {
                return dataFile;
            }
            set
            {
                dataFile = value;
                refreshView();
            }
        }

        public AgentSystem()
        {
            dataFile = new List<FileInfo>();
            allContractor = new List<Contractor>();
            isInitiator = false;
            countFinished = 0;
        }
        public void AddListener(setStat t) // добавить слушателя изменения статуса машины
        {
            infoMe.statusChange += t;
        }
        public void RemoveListener(setStat t) // удалить слушателя изменения статуса машины
        {
            infoMe.statusChange -= t;
        }

        public void TestSystem() // тестирование системы
        {
            infoMe.Status = StatusMachine.Testing; // начало тестирования
            infoMe.id = getMachineGuid().GetHashCode(); // получение хеш-кода GUID 
            infoMe.vRam = 1;
            infoMe.vCPU = 2;
            Thread.Sleep(1000); // имитация теста
            InitConnect();      // инициализируем подключение
            infoMe.Status = StatusMachine.Free; // свободен           
        }
        private string getMachineGuid() // получение GUID 
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
                    object machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
                    return machineGuid.ToString();
                }
            }
        }
        public void LoadSettings() // загрузка настроек
        {
            Status = StatusMachine.LoadSettings;
            string buf;
            StreamReader sr = null;
            try // читаем данные с файла настроек
            {
                sr = new StreamReader("set.cfg");
                buf = sr.ReadLine();
                settingSystem.ip = IPAddress.Parse(buf);
                buf = sr.ReadLine();
                settingSystem.mask = IPAddress.Parse(buf);
                buf = sr.ReadLine();
                settingSystem.port = Convert.ToUInt16(buf);
                sr.Close();
            }
            catch (FormatException ex)
            {
                settingSystem.ip = IPAddress.Parse("0.0.0.0");
                settingSystem.mask = IPAddress.Parse("255.255.255.0");
                settingSystem.port = 0;
                sr.Close();
                throw ex;
            }
            catch (IOException ex)
            {
                settingSystem.ip = IPAddress.Parse("0.0.0.0");
                settingSystem.mask = IPAddress.Parse("255.255.255.0");
                settingSystem.port = 0;
                throw ex;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(e.Message, "Error1", MessageBoxButtons.OK);
            }
            Status = StatusMachine.Free;
        }
        public void NetworkSettingsChange() // сетевые настройки изменены
        {
            Status = StatusMachine.LoadSettings;
            try
            {
                this.initiator.Restart(); // перезапускаем сервер
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + " in networkSettingsChange");
            }
            Status = StatusMachine.Free;
        }
        public void InitConnect() // начальный запуск сервера
        {
            try
            {
                if (initiator == null)
                {
                    initiator = new Initiator(this); // 56000
                    initiator.newMessage += GetPacket;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + " in InitConnect");
            }
        }
        public void Stop() // остановить всё
        {
            initiator.Stop();
        }

        // работа с файлами
        public void AddDataFile(FileInfo file) // добавить файл данных
        {
            foreach (FileInfo i in dataFile) // ищем, есть ли уже такой файл
            {
                if (i.Name.CompareTo(file.Name) == 0) // если есть - игнорируем добавление
                    return;
            }
            dataFile.Add(file); // иначе добавляем и обновляем представление
            refreshView();
        }
        public void RemoveDataFile(string fileName) // удаляем файл данных
        {
            foreach (FileInfo i in dataFile) // если находим файл - удаляем
            {
                if (i.Name == fileName)
                {
                    dataFile.Remove(i);
                    break;
                }
            }
            refreshView();  // обновляем представление
        }
        public void ReplaseDataFile(string removedFileName, FileInfo pasteFile) // заменяем файл с заданным именем на новый файл
        {
            int k = 0;
            foreach (FileInfo i in dataFile) // удалем файл
            {
                if (i.Name == removedFileName)
                {
                    dataFile.Remove(i);
                    break;
                }
                k++;
            }
            foreach (FileInfo i in dataFile) // вставляем на его позицию новый
            {
                if (i.Name.CompareTo(pasteFile.Name) == 0)
                    return;
            }
            dataFile.Add(pasteFile);
            refreshView();  // обновляем представление
        }
        public List<FileInfo> GetAllDataFile() // получить список всех файлов данных
        {
            return dataFile;
        }

        public List<Contractor> GetAllContractor() // получить список всех клиентов
        {
            return allContractor;
        }

        // работа с клиентами в качетсве инициатора
        public void RefreshContractorList() // обновить список клиентов
        {
            Thread th = new Thread(delegate () // поток обновления
            {
                IPAddress cureIP;                           // очередной IP
                StringBuilder headIP = new StringBuilder(); // начало IP
                Byte[] ipInBytes = IP.GetAddressBytes();
                headIP.Append(ipInBytes[0].ToString()).Append(".").Append(ipInBytes[1].ToString()).Append(".").Append(ipInBytes[2].ToString()).Append("."); // на время пока маска 255.255.255.0
                foreach (Contractor i in allContractor) // закрываем все соединения, кроме тех, что уже выбраны
                {
                    if (i.Connected && i.Selected == false)
                        i.Close();
                    allContractor.Remove(i);
                }
                MessageBox.Show("Обновляем список");
                Parallel.For(2, 254, tail => // перебираем все адреса с 2 до 254
                {
                    cureIP = IPAddress.Parse(headIP.ToString() + tail.ToString()); // формируем конечный IP
                    try
                    {
                        //MessageBox.Show("Заход №"+tail);
                        TcpClient client = new TcpClient();

                        if (client.ConnectAsync(cureIP, Port).Wait(1500)) // пытаемся с ним соединиться в течение 1,5 секунды
                        {
                            allContractor.Add(new Contractor(this, client));                    // в случае удачи досбавляем в список исполнителей
                            allContractor[allContractor.Count - 1].newMessage += GetPacket;
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("В процессе обновления произошла ошибка "+ex.Message+"\nПодробности:\n"+e.ToString());
                    }

                });
                MessageBox.Show("Нажмите ОК, чтоб закончить обновление списка");
            });
            th.IsBackground = true;
            th.Start();
            th.Join();
            refreshView();          // обновить отображение;
        }
        public void ConnectToContractor(IPAddress cureIP)
        {
            TcpClient client = new TcpClient();
            client.Connect(cureIP, Port); // пытаемся с ним соединиться в течение 15 секунд
            if (client.Connected)
            {
                allContractor.Add(new Contractor(this, client));                    // в случае удачи досбавляем в список исполнителей
                allContractor[allContractor.Count - 1].newMessage += GetPacket;
                refreshView();
            }
        }
        public void SelectContractor(int n) // выбрать для вычислений машину n
        {
            int i = -1;
            foreach (Contractor t in allContractor)
            {
                if (t.Selected == false)
                    i++;
                if (i == n)
                {
                    t.Selected = true;
                    break;
                }
            }
            refreshView();          // обновить отображение;
        }
        public void UnSelectContractor(int n) // снять выбор с машины n для вычислений
        {
            int i = -1;
            foreach (Contractor t in allContractor)
            {
                if (t.Selected == true)
                    i++;
                if (i == n)
                {
                    t.Selected = false;
                    break;
                }
            }
            refreshView();          // обновить отображение;
        }
        public void UnSelectAll() // снять выбор со всех машин
        {
            foreach (Contractor t in allContractor)
            {
                t.Selected = false;
            }
            refreshView();          // обновить отображение;
        }
        public void EndCalculate() // освобождаем всех, кого занимали
        {
            foreach (var t in allContractor)
                t.Close();
            allContractor.Clear();
            initiator.Restart();
        }

        void CreateFileIP() // собрать файл из всех IP в системе
        {
            FileInfo ipfile = new FileInfo("iplist.txt");
            if (ipfile.Exists) // если файл уже есть - пересоздать
                ipfile.Delete();
            StreamWriter sw = new StreamWriter(ipfile.Create());
            sw.WriteLine(IP); // записываем IP инцииатора
            foreach (var t in allContractor) // IP исполнителей
            {
                sw.WriteLine(t.GetIpServer().ToString());
            }
            sw.Close();
            AddDataFile(ipfile); // добавляем файл в список файлов данных
        }
        public void StartCalculate(bool startInInitiator) // запуск вычислений
        {
            countFinished = 0;
            isInitiator = true;
            isCalculate = true;
            refreshView();
            CreateFileIP(); // получаем сипсок машин
            foreach (var t in allContractor) // всем выбранным отправляем файлы
            {
                if (t.Selected == true)
                    t.SetExeAndDataFile();
                else
                    t.Close();
            }

            // запустить удаленно EXE
            foreach (var t in allContractor) // всем выбранным даем команду начать выполнение
            {
                t.SendMessage(new Packet() { type = PacketType.StartCalc, id = infoMe.id });
            }
            if (startInInitiator == true)
                runExe();
        }
        public void BreakCalculate() // обрыв вычислений
        {

        }

        // работа с инициатором в качестве клиента
        void runExe() // запускаем полученный или имеющийся exe
        {
            Status = StatusMachine.Calculate;
            Process proc = new Process();
            Thread.Sleep(1000);
            proc.StartInfo.FileName = exeFile.FullName;
            proc.StartInfo.WorkingDirectory = exeFile.Directory.ToString();
            proc.EnableRaisingEvents = true;
            proc.Exited += new EventHandler(endProc);
            proc.Start();
        }
        // по завершению работы программы
        void endProc(object sender, EventArgs e) // отправляем результаты и удаляем все полученные ранее файлы
        {
            try
            {
                Thread.Sleep(1000);
                if (isInitiator == false)
                {
                    Status = StatusMachine.UploadResult;
                    initiator.SendMessage(new Packet() { type = PacketType.FinishCalc, id = infoMe.id }); // отправляем сообщение о завершении вычислений                
                    exeFile.Delete();  // удаляем ранее полученные файлы
                    exeFile = null;
                    foreach (var t in dataFile)
                        t.Delete();
                    dataFile.Clear();
                }
                Status = StatusMachine.Free;
            }
            catch (Exception ex)
            {
                MessageBox.Show("endProc потерялся с сообщением " + ex.Message); // отладочный вывод
                MessageBox.Show("endProc потерялся из-за " + ex.Source); // отладочный вывод
                //MessageBox.Show("endProc потерялся  " + ex.ToString()); // отладочный вывод
            }
        }
        // работа с пакетами
        void GetPacket(ILockeded sender, string message) // обработка полученных пакетов
        {
            Packet pkt = new Packet() { type = PacketType.Empty, id = -1 };
            if (message.CompareTo("ConnectToInit") == 0)
                Status = StatusMachine.Wait;
            if (pkt.Parse(message) == true)
            {
                MessageBox.Show("Код " + pkt.type.ToString() + " | id " + pkt.id.ToString());
                switch (pkt.type)
                {
                    case PacketType.Hello:
                        sender.Locked = false;
                        break;
                    case PacketType.Run:
                        initiator.GetExeAndDataFile(true);
                        sender.Locked = false;
                        break;
                    case PacketType.Data:
                        initiator.GetExeAndDataFile(false);
                        sender.Locked = false;
                        break;
                    case PacketType.FinishCalc:
                        countFinished++;
                        if (countFinished == allContractor.Count)
                        {
                            foreach (var t in allContractor)
                                t.SendMessage(new Packet() { type = PacketType.Free, id = infoMe.id });
                            allContractor.Clear();
                            isCalculate = false;
                            refreshView();
                        }
                        sender.Locked = false;
                        break;
                    case PacketType.StartCalc:
                        sender.Locked = false;
                        runExe();
                        break;
                    case PacketType.StopCalc:
                        sender.Locked = false;
                        break;
                    case PacketType.Free:
                        Status = StatusMachine.Free;
                        initiator.Restart();
                        sender.Locked = false;
                        break;
                    case PacketType.Empty:
                        sender.Locked = false;
                        break;
                }
            }
        }
    }
}
