﻿using Agent.Enums;
using Agent.Structs;
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
        public event RefreshData RefreshView;
        public event UpdateProgressBar UpdProgress;

        private bool isInitiator=false; // Является ли инициатором
        private bool isCalculate=false; // начались ли вычисления
        private bool notDeleteFiles = false; // Не удалять файлы
        private bool endSplitFile = true; // закончили деление файла
        internal bool refreshContractor = false; // обновляется список исполнителей
        public int AllTime { get; private set; }
        public int UploadTime { get; private set; }
        public int CalculateTime { get; private set; }
        int countFinished = 0;              // количество завершивших вычисления
        private FileInfo exeFile;           // исполняемый файл
        private List<DiffFile> diffDataFile = new List<DiffFile>(); // разделяемые файлы
        private List<FileInfo> notDiffDataFile = new List<FileInfo>(); // не разделяемые файлы
        private List<Contractor> allContractor = new List<Contractor>();  // список всех клиентов (для работы в качестве инициатора)
        private Initiator initiator;            // информация о инициаторе
        private Process mainProc;           // основной процесс вычислений

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
                    RefreshView();
            }
        }
        public List<DiffFile> DiffDataFile
        {
            get
            {
                return diffDataFile;
            }
        }
        public List<FileInfo> NotDiffDataFile
        {
            get
            {
                return notDiffDataFile;
            }
        }

        // Запуск системы и добавление/удаление слушателей системы
        public AgentSystem() { }
        internal void AddListener(SetStat listener) // добавить слушателя изменения статуса машины
        {
            infoMe.StatusChange += listener;
        }
        internal void RemoveListener(SetStat listener) // удалить слушателя изменения статуса машины
        {
            infoMe.StatusChange -= listener;
        }
        
        // Тестирование системы и инициализация подключения
        public void TestSystem() // тестирование системы
        {
            infoMe.Status = StatusMachine.Testing; // начало тестирования
            infoMe.id = GetMachineGuid().GetHashCode(); // получение хеш-кода GUID 
            infoMe.vRam = GetMachineRAM(); // получение доступного объема RAM
            infoMe.vCPU = GetMachineCPUMHz(); // получаение тактовой частоты процессора
            if(Properties.Settings.Default.Port!=0)
                InitConnect();      // инициализируем подключение
            infoMe.Status = StatusMachine.Free; // свободен           
        }
        private string GetMachineGuid() // получение GUID 
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
                    return machineGuid.ToString();
                }
            }
        }
        private int GetMachineCPUMHz() // Определение тактовой частоты процессора
        {
            string location = @"HARDWARE\DESCRIPTION\System\CentralProcessor\0";
            string name = "~MHz";

            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
                    object machineGuid = rk.GetValue(name);
                    return int.Parse(machineGuid.ToString());
                }
            }
        }
        private int GetMachineRAM() // тест доступного объема RAM
        {
            int count = 0;
            const int MB = 1024 * 1024;
            int step = 32; // 32 МБ
            int maxSize = 32768; // 32ГБ
            try
            {
                
                byte[][] arr = new byte[maxSize/step][];
                for (int i = 0; i < maxSize / step; i++)
                {
                    arr[i] = new byte[MB * step];
                    count += step;
                }
            }
            catch(OutOfMemoryException)
            {
                return count;
            }
            return count;
        }
        public void NetworkSettingsChange() // сетевые настройки изменены
        {
            Status = StatusMachine.LoadSettings;
            if (this.initiator != null)
                this.initiator.Restart(); // перезапускаем сервер
            else
                this.InitConnect();
            Status = StatusMachine.Free;
        }
        public void InitConnect() // начальный запуск сервера
        {
            if (initiator == null)
            {
                initiator = new Initiator(this); // 56000
                initiator.NewMessage += GetPacket;
            }
        }
        public void Stop() // остановить всё
        {
            initiator.Stop();
        }

        // работа с делимыми файлами
        internal void AddDiffDataFile(FileInfo file, FileInfo exe) // добавить файл данных
        {
            foreach (var t in diffDataFile) // ищем, есть ли уже такой файл
            {
                if (t.data.Name.CompareTo(file.Name) == 0) // если есть - игнорируем добавление
                    return;
            }
            diffDataFile.Add(new DiffFile() { data = file, splitExe=exe }); // иначе добавляем и обновляем представление
            RefreshView();
        }
        internal void RemoveDiffDataFile(string fileName) // удаляем файл данных
        {
            foreach (var t in diffDataFile) // если находим файл - удаляем
            {
                if (t.data.Name == fileName)
                {
                    diffDataFile.Remove(t);
                    break;
                }
            }
            RefreshView();  // обновляем представление
        }
        internal void ReplaceDiffDataFile(string removedFileName, FileInfo pasteFile, FileInfo pasteExe) // заменяем файл с заданным именем на новый файл
        {
            foreach (var t in diffDataFile) // удалем файл
            {
                if (t.data.Name == removedFileName)
                {
                    diffDataFile.Remove(t);
                    break;
                }
            }
            foreach (var t in diffDataFile) // вставляем на его позицию новый
            {
                if (t.data.Name.CompareTo(pasteFile.Name) == 0)
                    return;
            }
            diffDataFile.Add(new DiffFile() { data=pasteFile, splitExe=pasteExe });
            RefreshView();  // обновляем представление
        }
        internal List<DiffFile> GetAllDiffDataFile() // получить список всех файлов данных
        {
            return diffDataFile;
        }
        // работа с не делимыми файлами
        internal void AddNotDiffDataFile(FileInfo file) // добавить файл данных
        {
            foreach (FileInfo i in notDiffDataFile) // ищем, есть ли уже такой файл
            {
                if (i.Name.CompareTo(file.Name) == 0) // если есть - игнорируем добавление
                    return;
            }
            notDiffDataFile.Add(file); // иначе добавляем и обновляем представление
            RefreshView();
        }
        internal void RemoveNotDiffDataFile(string fileName) // удаляем файл данных
        {
            foreach (FileInfo i in notDiffDataFile) // если находим файл - удаляем
            {
                if (i.Name == fileName)
                {
                    notDiffDataFile.Remove(i);
                    break;
                }
            }
            RefreshView();  // обновляем представление
        }
        internal void ReplaceNotDiffDataFile(string removedFileName, FileInfo pasteFile) // заменяем файл с заданным именем на новый файл
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
            RefreshView();  // обновляем представление
        }
        internal List<FileInfo> GetAllNotDiffDataFile() // получить список всех файлов данных
        {
            return notDiffDataFile;
        }

        internal List<Contractor> GetAllContractor() // получить список всех клиентов
        {
            return allContractor;
        }
        internal int GetCountSelectredContractor()
        {
            int count=0;
            foreach (var t in allContractor)
                if (t.Selected == true)
                    count++;
            return count;
        }

        // работа с клиентами в качетсве инициатора
        internal void RemoveContractor(Contractor temp)
        {
            allContractor.Remove(temp);
            RefreshView();
        }
        internal void RefreshContractorList() // обновить список клиентов
        {
            string name = "Обновсление списка исполнителей";
            int countEnd = 0;
            int countAll = 254;
            refreshContractor = true;
            RefreshView();
            UpdProgress(countEnd, countAll, name);
            Thread th = new Thread(delegate () // поток обновления
            {
                lock (allContractor)
                {
                    IPAddress cureIP;                           // очередной IP
                    StringBuilder headIP = new StringBuilder(); // начало IP
                    string[] ipInBytes = Properties.Settings.Default.IP.Split('.');
                    headIP.Append(ipInBytes[0]).Append(".").Append(ipInBytes[1]).Append(".").Append(ipInBytes[2]).Append("."); // на время пока маска 255.255.255.0
                    Log.Write("Запущено обновление списка");
                    Parallel.For(2, 254, tail => // перебираем все адреса с 2 до 254
                    {
                        cureIP = IPAddress.Parse(headIP.ToString() + tail.ToString()); // формируем конечный IP
                        try
                        {
                            TcpClient client = new TcpClient();

                            if (client.ConnectAsync(cureIP, Properties.Settings.Default.Port).Wait(1500)) // пытаемся с ним соединиться в течение 1,5 секунды
                            {
                                allContractor.Add(new Contractor(this, client));                    // в случае удачи досбавляем в список исполнителей
                                allContractor[allContractor.Count - 1].NewMessage += GetPacket;
                                RefreshView();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }
                        countEnd++;
                        UpdProgress(countEnd, countAll, name);
                    });
                    Log.Write("Список обновлен");
                    refreshContractor = false;
                }
                RefreshView();
                UpdProgress(countEnd, countAll, "Обновление завершено");
            });
            th.IsBackground = true;
            th.Start();
        }
        internal void ConnectToContractor(IPAddress cureIP)
        {
            TcpClient client = new TcpClient();
            client.Connect(cureIP, Properties.Settings.Default.Port); // пытаемся с ним соединиться в течение 15 секунд
            if (client.Connected)
            {
                allContractor.Add(new Contractor(this, client));                    // в случае удачи досбавляем в список исполнителей
                allContractor[allContractor.Count - 1].NewMessage += GetPacket;
                RefreshView();
            }
        }
        internal void SelectContractor(int n) // выбрать для вычислений машину n
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
        internal void UnSelectContractor(int n) // снять выбор с машины n для вычислений
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
        internal void UnSelectAll() // снять выбор со всех машин
        {
            foreach (Contractor t in allContractor)
            {
                t.Selected = false;
            }
        }
        internal void EndCalculate() // освобождаем всех, кого занимали
        {
            foreach (var t in allContractor)
                t.Close();
            allContractor.Clear();
            initiator.Restart();
        }

        // Работа с файлами для запуска вычислений
        void CopyFile(FileInfo file, string fullName) // копирование файла
        {
            file.CopyTo(fullName, true);
        }
        void CopyFilesInitiator() // копирование всех файлов в директорию Temp
        {
            string name = "Копирование файлов на инициаторе";
            int countCopy = 0;
            int countAllFiles = DiffDataFile.Count*2 + NotDiffDataFile.Count + 1;
            UpdProgress(countCopy,countAllFiles, name);
            List<DiffFile> newDiffDataList = new List<DiffFile>();
            List<FileInfo> newNotDiffDataList = new List<FileInfo>();
            string source = Application.StartupPath + "\\Temp\\";
            DirectoryInfo DI = new DirectoryInfo(Application.StartupPath + "\\Temp\\");
            DI.Create();
            CopyFile(exeFile, source + exeFile.Name);
            countCopy++;
            UpdProgress(countCopy, countAllFiles, name);
            exeFile = new FileInfo(source + exeFile.Name);
            foreach (var t in notDiffDataFile)
            {
                CopyFile(t, source + t.Name);
                countCopy++;
                UpdProgress(countCopy, countAllFiles, name);
                newNotDiffDataList.Add(new FileInfo(source + t.Name));
            }
            notDiffDataFile.Clear();
            notDiffDataFile = newNotDiffDataList;
            foreach (var t in diffDataFile)
            {
                CopyFile(t.data, source + t.data.Name);
                countCopy++;
                UpdProgress(countCopy, countAllFiles, name);
                CopyFile(t.splitExe, source + t.splitExe.Name);
                countCopy++;
                UpdProgress(countCopy, countAllFiles, name);
                newDiffDataList.Add(new DiffFile() { data = new FileInfo(source + t.data.Name), splitExe = new FileInfo(source + t.splitExe.Name) });
            }
            diffDataFile.Clear();
            diffDataFile = newDiffDataList;
            UpdProgress(1,1, "Данные на инициаторе скопированы");
        }
        void SplitOneFile(DiffFile file, int countParts) // разделение одного файла
        {
            Process splitter = new Process();
            splitter.StartInfo = new ProcessStartInfo(file.splitExe.FullName, file.data.Name + " " + countParts);
            splitter.EnableRaisingEvents = true;
            splitter.Exited += new EventHandler(EndSplitOneFile);
            splitter.StartInfo.WorkingDirectory = Application.StartupPath + "\\TEMP";
            splitter.Start();
        }
        void EndSplitOneFile(object sender, EventArgs e) 
        {
            Log.Write("Деление файла завершено");
            endSplitFile = true;
        }
        void ComplectFileParts(FileInfo file, int countParts) // Распределение частей файла по исполнителям
        {
            string PathName= Application.StartupPath + "\\TEMP\\"+file.Name.Remove(file.Name.LastIndexOf('.'));
            for(int i=1; i<countParts; i++)
            {
                allContractor[i - 1].AddFile(new FileInfo(PathName + "\\" + i + "\\" + file.Name));
            }
            CopyFile(new FileInfo(PathName + "\\" + 0 + "\\" + file.Name), Application.StartupPath + "\\Temp\\"+ file.Name);
        }
        void DiffFileList(List<DiffFile> files, int countParts) // разделение набора файлов
        {
            string name = "Деление файлов: ";
            int count=0, countAll= diffDataFile.Count*2;
            if (diffDataFile.Count != 0)
            {
                foreach (var t in files)
                {
                    UpdProgress(count, diffDataFile.Count, name + (count+1) + " из " + countAll);
                    endSplitFile = false;
                    SplitOneFile(t, countParts);
                    while (endSplitFile != true) ;
                    count++;
                    UpdProgress(count, diffDataFile.Count, "Комплектация файла " + (count + 1) + " из " + countAll);
                    ComplectFileParts(t.data, countParts);
                    count++;
                }
            }
            UpdProgress(1, 1, "Деление файлов завершено");
        }
        void CreateFileIP() // собрать файл из всех IP в системе
        {
            int countMachines;
            FileInfo ipfile = new FileInfo("TEMP\\iplist.txt");
            if (ipfile.Exists) // если файл уже есть - пересоздать
                ipfile.Delete();
            StreamWriter sw = new StreamWriter(ipfile.Create());
            countMachines = allContractor.Count + 1; // количество машин в подсистеме
            sw.WriteLine(countMachines);
            sw.WriteLine(Properties.Settings.Default.IP); // записываем IP инцииатора
            foreach (var t in allContractor) // IP исполнителей
            {
                sw.WriteLine(t.GetIPServer().ToString());
            }
            sw.Close();
            AddNotDiffDataFile(ipfile); // добавляем файл в список файлов данных
        }
        void UploadFiles() // деление и распределение файлов
        {
            UploadTime = Environment.TickCount;
            string name = "Делим файлы и создаем iplist";
            int count, countAll;
            UpdProgress(0,2, name);
            Thread calcThread = new Thread(delegate () {
                CreateFileIP(); // получаем сипсок машин
                UpdProgress(1,2, name);
                DiffFileList(diffDataFile, allContractor.Count + 1);
                UpdProgress(2,2, name);
                name = "Отправка файлов исполнителю ";
                count = 0;
                countAll = allContractor.Count;
                foreach (var t in allContractor) // всем отправляем файлы
                {
                    UpdProgress(count , countAll, name + (count + 1) + "из" + countAll);
                    if (notDeleteFiles)
                        t.SendMessage(new Packet() { type = PacketType.NotDeleteFiles, id = infoMe.id });
                    t.AddFileList(notDiffDataFile); // добавляем к отправке не делимые файлы (в т.ч. iplist.txt)
                                                    // TODO: добавление части
                    t.SetExeAndDataFile(); // отправляем файлы
                    count++;
                }
                // запустить удаленно EXE
                name = "Запускаем вычисления";
                count = 0;
                countAll += 1;
                UpdProgress(count , countAll, name);
                foreach (var t in allContractor) // всем выбранным даем команду начать выполнение
                {
                    t.SendMessage(new Packet() { type = PacketType.StartCalc, id = infoMe.id });
                    count++;
                    UpdProgress(count, countAll, name);
                }
                RunExe();
                count++;
                UpdProgress(count, countAll, "Вычисления запущены");
            });
            calcThread.IsBackground = true;
            calcThread.Start();
            UploadTime = Environment.TickCount - UploadTime;
            CalculateTime = Environment.TickCount;
        }

        // Запуск и обрыв вычислений
        internal void StartCalculate(bool notDeleteFiles) // запуск вычислений
        {
            AllTime = Environment.TickCount;
            List<Contractor> removed = new List<Contractor>();
            countFinished = 0;
            isInitiator = true;
            isCalculate = true;
            this.notDeleteFiles = notDeleteFiles;
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
            CopyFilesInitiator();
            RefreshView();
            UploadFiles();
        }
        internal void BreakCalculate() // обрыв вычислений
        {
            mainProc.Kill();
            if(isInitiator==true)
            {
                foreach (var t in allContractor)
                    t.SendMessage(new Packet() { id=infoMe.id, type=PacketType.StopCalc });
                allContractor.Clear();
                isCalculate = false;
                UpdProgress(2, 2, "Вычисления прерваны");
                RefreshView();
            }
            if (notDeleteFiles == false)
            {
                Directory.Delete(Application.StartupPath + "\\Temp", true);
                exeFile = null;
                notDiffDataFile.Clear();
                diffDataFile.Clear();
            }
        }

        // работа с инициатором в качестве клиента
        void RunExe() // запускаем полученный или имеющийся exe
        {
            Status = StatusMachine.Calculate;
            mainProc = new Process();
            Thread.Sleep(1000);
            mainProc.StartInfo.FileName = exeFile.FullName;
            mainProc.StartInfo.WorkingDirectory = exeFile.Directory.ToString();
            mainProc.EnableRaisingEvents = true;
            mainProc.Exited += new EventHandler(EndProc);
            mainProc.Start();
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
                else
                {
                    UpdProgress(1,2, "Вычисления завершаются");
                    if(allContractor.Count==0)
                    {
                        isCalculate = false;
                        UpdProgress(2,2, "Вычисления завершены");
                        RefreshView();
                        CalculateTime = Environment.TickCount - CalculateTime;
                        AllTime = Environment.TickCount - AllTime;
                        Log.ShowMessage("Общее время вычислений: " + AllTime/1000 + " сек\nВремя на передачу файлов: " + UploadTime / 1000 + " сек\nВремя рассчетов: " + CalculateTime / 1000 + " сек");
                    }
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
                            CalculateTime = Environment.TickCount - CalculateTime;
                            AllTime = Environment.TickCount - AllTime;
                            isCalculate = false;
                            UpdProgress(2,2, "Вычисления завершены");
                            RefreshView();
                            Log.ShowMessage("Общее время вычислений: " + AllTime / 1000 + " сек\nВремя на передачу файлов: " + UploadTime / 1000 + " сек\nВремя рассчетов: " + CalculateTime / 1000 + " сек");
                        }
                        sender.Locked = false;
                        break;
                    case PacketType.StartCalc:
                        sender.Locked = false;
                        RunExe();
                        break;
                    case PacketType.StopCalc:
                        BreakCalculate();
                        Programm.Reset();
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
