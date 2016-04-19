using Microsoft.Win32;
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

        private bool isInitiator=false; // Является ли инициатором
        private bool isCalculate=false; // начались ли вычисления
        private bool notDeleteFiles = false; // Не удалять файлы
        public bool refreshContractor = false; // обновляется список исполнителей
        int countFinished = 0;              // количество завершивших вычисления
        private FileInfo exeFile;           // исполняемый файл
        private List<FileInfo> diffDataFile = new List<FileInfo>(); // разделяемые файлы
        private List<FileInfo> notDiffDataFile = new List<FileInfo>(); // не разделяемые файлы
        private List<Contractor> allContractor = new List<Contractor>();  // список всех клиентов (для работы в качестве инициатора)
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
                        if (infoMe.Status == StatusMachine.Free)
                        {
                            initiator.Start();
                            isInitiator = false;
                        }
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
        public List<FileInfo> DiffDataFile
        {
            get
            {
                return diffDataFile;
            }
            set
            {
                diffDataFile = value;
                refreshView();
            }
        }
        public List<FileInfo> NotDiffDataFile
        {
            get
            {
                return notDiffDataFile;
            }
            set
            {
                notDiffDataFile = value;
                refreshView();
            }
        }

        public AgentSystem() { }
        public void AddListener(setStat t) // добавить слушателя изменения статуса машины
        {
            infoMe.statusChange += t;
        }
        public void RemoveListener(setStat t) // удалить слушателя изменения статуса машины
        {
            infoMe.statusChange -= t;
        }

        void CopyFile(FileInfo file, string name) // копирование файла
        {
            file.CopyTo(name, true);
        }
        void CopyFilesInitiator() // копирование всех файлов в директорию Temp
        {
            List<FileInfo> newDiffDataList = new List<FileInfo>();
            List<FileInfo> newNotDiffDataList = new List<FileInfo>();
            string source = Application.StartupPath + "\\Temp\\";
            DirectoryInfo DI = new DirectoryInfo(Application.StartupPath + "\\Temp\\");
            DI.Create();
            CopyFile(exeFile, source + exeFile.Name);
            exeFile = new FileInfo(source + exeFile.Name);
            foreach (var t in notDiffDataFile)
            {
                CopyFile(t, source + t.Name);
                newNotDiffDataList.Add(new FileInfo(source + t.Name));
            }
            notDiffDataFile.Clear();
            notDiffDataFile = newNotDiffDataList;
            foreach (var t in diffDataFile)
            {
                CopyFile(t, source + t.Name);
                newDiffDataList.Add(new FileInfo(source + t.Name));
            }
            notDiffDataFile = newDiffDataList;
        }

        public void TestSystem() // тестирование системы
        {
            infoMe.Status = StatusMachine.Testing; // начало тестирования
            infoMe.id = getMachineGuid().GetHashCode(); // получение хеш-кода GUID 
            infoMe.vRam = (new Random()).Next(100,200);
            infoMe.vCPU = (new Random()).Next(200,250);
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
            catch (Exception ex)
            {
                if (ex is FormatException)
                    sr.Close();
                settingSystem.ip = IPAddress.Parse("127.0.0.1");
                settingSystem.mask = IPAddress.Parse("255.255.255.0");
                settingSystem.port = 0;
                throw ex;
            }
            Status = StatusMachine.Free;
        }
        public void NetworkSettingsChange() // сетевые настройки изменены
        {
            Status = StatusMachine.LoadSettings;
            this.initiator.Restart(); // перезапускаем сервер
            Status = StatusMachine.Free;
        }
        public void InitConnect() // начальный запуск сервера
        {
            if (initiator == null)
            {
                initiator = new Initiator(this); // 56000
                initiator.newMessage += GetPacket;
            }
        }
        public void Stop() // остановить всё
        {
            initiator.Stop();
        }

        // работа с делимыми файлами
        public void AddDiffDataFile(FileInfo file) // добавить файл данных
        {
            foreach (FileInfo i in diffDataFile) // ищем, есть ли уже такой файл
            {
                if (i.Name.CompareTo(file.Name) == 0) // если есть - игнорируем добавление
                    return;
            }
            diffDataFile.Add(file); // иначе добавляем и обновляем представление
            refreshView();
        }
        public void RemoveDiffDataFile(string fileName) // удаляем файл данных
        {
            foreach (FileInfo i in diffDataFile) // если находим файл - удаляем
            {
                if (i.Name == fileName)
                {
                    diffDataFile.Remove(i);
                    break;
                }
            }
            refreshView();  // обновляем представление
        }
        public void ReplaceDiffDataFile(string removedFileName, FileInfo pasteFile) // заменяем файл с заданным именем на новый файл
        {
            int k = 0;
            foreach (FileInfo i in diffDataFile) // удалем файл
            {
                if (i.Name == removedFileName)
                {
                    diffDataFile.Remove(i);
                    break;
                }
                k++;
            }
            foreach (FileInfo i in diffDataFile) // вставляем на его позицию новый
            {
                if (i.Name.CompareTo(pasteFile.Name) == 0)
                    return;
            }
            diffDataFile.Add(pasteFile);
            refreshView();  // обновляем представление
        }
        public List<FileInfo> GetAllDiffDataFile() // получить список всех файлов данных
        {
            return diffDataFile;
        }
        // работа с не делимыми файлами
        public void AddNotDiffDataFile(FileInfo file) // добавить файл данных
        {
            foreach (FileInfo i in notDiffDataFile) // ищем, есть ли уже такой файл
            {
                if (i.Name.CompareTo(file.Name) == 0) // если есть - игнорируем добавление
                    return;
            }
            notDiffDataFile.Add(file); // иначе добавляем и обновляем представление
            refreshView();
        }
        public void RemoveNotDiffDataFile(string fileName) // удаляем файл данных
        {
            foreach (FileInfo i in notDiffDataFile) // если находим файл - удаляем
            {
                if (i.Name == fileName)
                {
                    notDiffDataFile.Remove(i);
                    break;
                }
            }
            refreshView();  // обновляем представление
        }
        public void ReplaceNotDiffDataFile(string removedFileName, FileInfo pasteFile) // заменяем файл с заданным именем на новый файл
        {
            int k = 0;
            foreach (FileInfo i in notDiffDataFile) // удалем файл
            {
                if (i.Name == removedFileName)
                {
                    notDiffDataFile.Remove(i);
                    break;
                }
                k++;
            }
            foreach (FileInfo i in notDiffDataFile) // вставляем на его позицию новый
            {
                if (i.Name.CompareTo(pasteFile.Name) == 0)
                    return;
            }
            notDiffDataFile.Add(pasteFile);
            refreshView();  // обновляем представление
        }
        public List<FileInfo> GetAllNotDiffDataFile() // получить список всех файлов данных
        {
            return notDiffDataFile;
        }

        public List<Contractor> GetAllContractor() // получить список всех клиентов
        {
            return allContractor;
        }

        // работа с клиентами в качетсве инициатора
        public void RefreshContractorList() // обновить список клиентов
        {
            refreshContractor = true;
            refreshView();
            Thread th = new Thread(delegate () // поток обновления
            {
                lock (allContractor)
                {
                    IPAddress cureIP;                           // очередной IP
                    StringBuilder headIP = new StringBuilder(); // начало IP
                    Byte[] ipInBytes = IP.GetAddressBytes();
                    headIP.Append(ipInBytes[0].ToString()).Append(".").Append(ipInBytes[1].ToString()).Append(".").Append(ipInBytes[2].ToString()).Append("."); // на время пока маска 255.255.255.0
                    foreach (var t in allContractor) // закрываем все соединения
                    {
                        if (t.Connected)
                            t.Close();
                    }
                    allContractor.Clear();
                    Programm.ShowMessage("Обновляем список");
                    Parallel.For(2, 254, tail => // перебираем все адреса с 2 до 254
                    {
                        cureIP = IPAddress.Parse(headIP.ToString() + tail.ToString()); // формируем конечный IP
                        try
                        {
                            //Programm.ShowMessage("Заход №"+tail);
                            TcpClient client = new TcpClient();

                            if (client.ConnectAsync(cureIP, Port).Wait(1500)) // пытаемся с ним соединиться в течение 1,5 секунды
                            {
                                allContractor.Add(new Contractor(this, client));                    // в случае удачи досбавляем в список исполнителей
                                allContractor[allContractor.Count - 1].newMessage += GetPacket;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }

                    });
                    Programm.ShowMessage("Список обновлен");
                    refreshContractor = false;
                }
                refreshView();
            });
            th.IsBackground = true;
            th.Start();
            //refreshView();          // обновить отображение;
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
        }
        public void UnSelectAll() // снять выбор со всех машин
        {
            foreach (Contractor t in allContractor)
            {
                t.Selected = false;
            }
        }
        public void EndCalculate() // освобождаем всех, кого занимали
        {
            foreach (var t in allContractor)
                t.Close();
            allContractor.Clear();
            initiator.Restart();
        }

        // Работа с файлами для запуска вычислений
        List<FileInfo> DiffOneFile(FileInfo file, int countParts) // разделение одного файла
        {
            List<FileInfo> parts = new List<FileInfo>();
            for(int i=0; i<countParts; i++)
            {
                // как-то делит
            }
            return parts;
        }
        List<List<FileInfo>> DiffFileList(List<FileInfo> files, int countParts) // разделение набора файлов
        {
            List<List<FileInfo>> fileParts = new List<List<FileInfo>>();
            if (diffDataFile.Count != 0)
            {
                
                DirectoryInfo DI = new DirectoryInfo("TEMP\\Parts");
                DI.Create();
                foreach (var t in files)
                {
                    fileParts.Add(DiffOneFile(t, countParts));
                }
            }
            return fileParts;
        }
        void CreateFileIP() // собрать файл из всех IP в системе
        {
            int countMachines;
            FileInfo ipfile = new FileInfo("iplist.txt");
            if (ipfile.Exists) // если файл уже есть - пересоздать
                ipfile.Delete();
            StreamWriter sw = new StreamWriter(ipfile.Create());
            countMachines = allContractor.Count + 1; // количество машин в подсистеме
            sw.WriteLine(countMachines);
            sw.WriteLine(IP); // записываем IP инцииатора
            foreach (var t in allContractor) // IP исполнителей
            {
                sw.WriteLine(t.GetIpServer().ToString());
            }
            sw.Close();
            AddNotDiffDataFile(ipfile); // добавляем файл в список файлов данных
        }
        
        // Запуск и обрыв вычислений
        public void StartCalculate(bool notDeleteFiles) // запуск вычислений
        {
            
            this.notDeleteFiles = notDeleteFiles;
            CopyFilesInitiator();
            countFinished = 0;
            isInitiator = true;
            isCalculate = true;
            List<Contractor> removed = new List<Contractor>();
            foreach (var t in allContractor) // Собираем список всех свободных
            {
                if (t.Selected == false)
                {
                    t.Close();
                    removed.Add(t);
                }
            }
            foreach(var t in removed) // Удаляем их из списка
            {
                allContractor.Remove(t);
            }
            refreshView();
            Thread calcThread = new Thread(delegate () {
                CreateFileIP(); // получаем сипсок машин
                foreach (var t in allContractor) // всем отправляем файлы
                {
                    if (notDeleteFiles)
                        t.SendMessage(new Packet() { type = PacketType.NotDeleteFiles, id = infoMe.id });
                    t.AddFileList(notDiffDataFile); // добавляем к отправке не делимые файлы (в т.ч. iplist.txt)
                                                    // TODO: добавление части
                    t.SetExeAndDataFile(); // отправляем файлы
                }
                // запустить удаленно EXE
                foreach (var t in allContractor) // всем выбранным даем команду начать выполнение
                {
                    t.SendMessage(new Packet() { type = PacketType.StartCalc, id = infoMe.id });
                }
                RunExe();
            });
            calcThread.IsBackground = true;
            calcThread.Start();
        }
        public void BreakCalculate() // обрыв вычислений
        {

        }

        // работа с инициатором в качестве клиента
        void RunExe() // запускаем полученный или имеющийся exe
        {
            Status = StatusMachine.Calculate;
            Process proc = new Process();
            Thread.Sleep(1000);
            proc.StartInfo.FileName = exeFile.FullName;
            proc.StartInfo.WorkingDirectory = exeFile.Directory.ToString();
            proc.EnableRaisingEvents = true;
            proc.Exited += new EventHandler(EndProc);
            proc.Start();
        }
        // по завершению работы программы 
        void EndProc(object sender, EventArgs e) // отправляем результаты и удаляем все полученные ранее файлы
        {
            try
            {
                Thread.Sleep(1000);
                if (isInitiator == false)
                {
                    Status = StatusMachine.WaitEndCalc;
                    initiator.SendMessage(new Packet() { type = PacketType.FinishCalc, id = infoMe.id }); // отправляем сообщение о завершении вычислений    
                }
                if (notDeleteFiles == false)
                {
                    Directory.Delete(Application.StartupPath + "\\Temp", true);
                    exeFile = null;
                    notDiffDataFile.Clear();
                    diffDataFile.Clear();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
        // работа с пакетами
        void GetPacket(ILockeded sender, string message) // обработка полученных пакетов
        {
            Packet pkt = new Packet() { type = PacketType.Empty, id = -1 };
            string[] parts = message.Split(' ');
            if (Enum.TryParse(parts[0], out pkt.type)) // пытаемся пропарсить сообщение на соответствие пакету
            {
                pkt.id = long.Parse(parts[1]);
                Log.Write("Код " + pkt.type.ToString() + " | id " + pkt.id.ToString());
                switch (pkt.type)
                {
                    case PacketType.Hello:
                        Status = StatusMachine.Wait;
                        ((Initiator)sender).SendInfoMe();
                        sender.Locked = false;
                        break;
                    case PacketType.RunFile:
                        DirectoryInfo DI = new DirectoryInfo(Application.StartupPath + "\\Temp\\");
                        DI.Create();
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
                        RunExe();
                        break;
                    case PacketType.StopCalc:
                        sender.Locked = false;
                        break;
                    case PacketType.Free:
                        Status = StatusMachine.Free;
                        Programm.Reset();
                        sender.Locked = false;
                        break;
                    case PacketType.NotDeleteFiles:
                        notDeleteFiles = true;
                        sender.Locked = false;
                        break;
                    case PacketType.Empty:
                        sender.Locked = false;
                        break;
                }
            }
            else if (message.CompareTo("ConnectToInit") == 0)
                Status = StatusMachine.Wait;
        }
    }
}
