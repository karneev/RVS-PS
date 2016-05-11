using Agent.Enums;
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
        #region События системы
        public event RefreshData RefreshView;
        public event UpdateProgressBar UpdProgress;
        #endregion

        #region Основные переменные
        private bool isInitiator=false; // Является ли инициатором
        private bool isCalculate=false; // начались ли вычисления
        private bool notDeleteFiles = false; // Не удалять файлы
        private bool endSplitFile = true; // закончили деление файла
        internal bool refreshContractor = false; // обновляется список исполнителей
        public DateTime AllTimeStart { get; private set; }
        public DateTime AllTimeEnd { get; private set; }
        public DateTime UploadTimeStart { get; private set; }
        public DateTime UploadTimeEnd { get; private set; }
        public DateTime CalculateTimeStart { get; private set; }
        public DateTime CalculateTimeEnd { get; private set; }
        int countFinished = 0;              // количество завершивших вычисления
        private FileInfo exeFile;           // исполняемый файл
        private List<DiffFile> diffDataFile = new List<DiffFile>(); // разделяемые файлы
        private List<FileInfo> notDiffDataFile = new List<FileInfo>(); // не разделяемые файлы
        private List<Contractor> allContractor = new List<Contractor>();  // список всех клиентов (для работы в качестве инициатора)
        private Initiator initiator;            // информация о инициаторе
        private Process mainProc;           // основной процесс вычислений

        private MachineInfo infoMe;         // Информация о себе
        #endregion

        #region Поля-свойства
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
        #endregion

        #region Запуск системы и добавление/удаление слушателей
        public AgentSystem() { }
        internal void AddListener(SetStat listener) // добавить слушателя изменения статуса машины
        {
            infoMe.StatusChange += listener;
        }
        internal void RemoveListener(SetStat listener) // удалить слушателя изменения статуса машины
        {
            infoMe.StatusChange -= listener;
        }
        #endregion

        #region Тестирование системы и инициализация подключения
        public void TestSystem() // тестирование системы
        {
            infoMe.Status = StatusMachine.Testing; // начало тестирования
            infoMe.id = GetMachineGuid().GetHashCode(); // получение хеш-кода GUID 
            infoMe.vRam = GetMachineRAM(); // получение доступного объема RAM
            infoMe.vCPU = GetMachineCPUMHz(); // получаение тактовой частоты процессора
            if(Properties.Settings.Default.IP.CompareTo("127.0.0.1")!=0)
                InitConnect();      // инициализируем подключение
            infoMe.Status = StatusMachine.Free; // свободен           
        }
        public void UpdateAutoRun() // изменить состояние ключа
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (Properties.Settings.Default.AutoRun)
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("Agent", Application.ExecutablePath.ToString());
                rkApp.SetValue("Agent", "\"" + Application.ExecutablePath.ToString() + "\" -autorun");
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("Agent", false);
            }

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
        #endregion

        #region Работа с делимыми файлами
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
        #endregion

        #region Работа с неделимыми файлами
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
        #endregion

        #region Работа с исполнителями в качетсве инициатора
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
        
        internal void SaveAllContractorToDB()
        {
            SQLiteDriver DB = new SQLiteDriver();
            foreach (var t in allContractor)
            {
                DB.AddIP(t.GetIPServer().ToString());
            }
            DB.Close();
        }
        internal void LoadAllContractorFromDB()
        {
            Thread th = new Thread(delegate ()
            {
                int i = 0;
                SQLiteDriver DB = new SQLiteDriver();
                List<string> allIP = DB.GetAllIP();
                foreach (var t in allIP)
                {
                    ConnectToContractor(IPAddress.Parse(t), 1500);
                    UpdProgress(i++, allIP.Count, "Загрузка из базы данных");
                }
                DB.Close();
                UpdProgress(i++, allIP.Count, "Загрузка завершена");
            });
            th.IsBackground = true;
            th.Start();
        }
        internal void AddContractor(TcpClient client)
        {
            allContractor.Add(new Contractor(this, client));                    // в случае удачи досбавляем в список исполнителей
            allContractor[allContractor.Count - 1].NewMessage += GetPacket;
            RefreshView();
        }
        internal void RemoveContractor(Contractor temp)
        {
            allContractor.Remove(temp);
            RefreshView();
        }

        private byte[] IPToByteArray(string IP)
        {
            string[] ip = IP.Split('.');
            byte[] array = new byte[4];
            for (int i = 0; i < 4; i++)
                array[i] = Byte.Parse(ip[i]);
            return array;
        }
        private byte[] GetNetworkIP(byte[] pointIP, byte[] mask)
        {
            byte[] networkIP = new byte[4];
            for (int i = 0; i < 4; i++)
                networkIP[i] = (byte)(pointIP[i] & mask[i]);
            return networkIP;
        }
        private int GetCountIPinNetwork(byte[] mask)
        {
            return (256 - mask[0]) * (256 - mask[1]) * (256 - mask[2]) * (256 - mask[3]);
        }
        private bool IsCorrectIP(int a, int b, int c, int d, byte[] networkIP,out string cureIP)
        {
            cureIP = "0.0.0.0";
            if (a != 255 && b != 255 && c != 255 && d != 0 && d != 255) // Не является ли IP широковещательным
            {
                cureIP = (a + networkIP[0]).ToString() + "." + (b + networkIP[1]).ToString() + "." + (c + networkIP[2]).ToString() + "." + (d + networkIP[3]).ToString();
                return true;
            }
            else
                return false;
            
        }
        internal void RefreshContractorList() // обновить список клиентов
        {
            string name = "Обновсление списка исполнителей";
            int countEnd = 0;
            int countAll = 1;
            refreshContractor = true;
            RefreshView();
            UpdProgress(countEnd, countAll, name);
            Thread th = new Thread(delegate () // поток обновления
            {
                lock (allContractor)
                {
                    byte[] pointIP = IPToByteArray(Properties.Settings.Default.IP);
                    byte[] mask = IPToByteArray(Properties.Settings.Default.Mask);
                    byte[] networkIP = GetNetworkIP(pointIP, mask);
                    countAll = GetCountIPinNetwork(mask);

                    Log.Write("Запущено обновление списка");
                    // перебираем все адреса в соответствии с маской
                    Parallel.For(0, 256 - mask[0], a =>
                      {
                          Parallel.For(0, 256 - mask[1], b =>
                          {
                              Parallel.For(0, 256 - mask[2], c =>
                              {
                                  Parallel.For(0, 256 - mask[3], d => 
                                  {
                                      try
                                      {
                                          string cureIP;
                                          if (IsCorrectIP(a, b, c, d, networkIP, out cureIP))
                                              ConnectToContractor(IPAddress.Parse(cureIP), 1500);
                                      }
                                      catch (Exception ex)
                                      {
                                          Log.Write(ex);
                                      }
                                      countEnd++;
                                      UpdProgress(countEnd, countAll, name);
                                  });
                              });
                          });
                      });
                    Log.Write("Список обновлен");
                    refreshContractor = false;
                }
                RefreshView();
                SaveAllContractorToDB();
                UpdProgress(countEnd, countAll, "Обновление завершено");
            });
            th.IsBackground = true;
            th.Start();
        }
        internal void ConnectToContractor(IPAddress cureIP, int timeout)
        {
            try
            {
                TcpClient client = new TcpClient();
                if (client.ConnectAsync(cureIP, Properties.Settings.Default.Port).Wait(timeout)) // пытаемся соединиться с клиентом
                    AddContractor(client);
            }
            catch(Exception ex)
            {
                Log.Write(ex);
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
        #endregion

        #region Работа с файлами для запуска вычислений
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
            UploadTimeStart = DateTime.Now;
            string name = "Делим файлы и создаем iplist";
            int count, countAll;
            UpdProgress(0, 2, name);
            Thread calcThread = new Thread(delegate () {
                CreateFileIP(); // получаем сипсок машин
                UpdProgress(1, 2, name);
                DiffFileList(diffDataFile, allContractor.Count + 1);
                UpdProgress(2, 2, name);
                name = "Отправка файлов исполнителю ";
                count = 0;
                countAll = allContractor.Count;
                foreach (var t in allContractor) // всем отправляем файлы
                {
                    UpdProgress(count, countAll, name + (count + 1) + "из" + countAll);
                    if (notDeleteFiles)
                        t.SendMessage(new Packet() { type = PacketType.NotDeleteFiles, id = infoMe.id });
                    t.AddFileList(notDiffDataFile); // добавляем к отправке неделимые файлы (в т.ч. iplist.txt)
                                                    // TODO: добавление части
                    t.SetExeAndDataFile(); // отправляем файлы
                    count++;
                }
                // запустить удаленно EXE
                name = "Запускаем вычисления";
                count = 0;
                countAll += 1;
                UpdProgress(count, countAll, name);
                foreach (var t in allContractor) // всем выбранным даем команду начать выполнение
                {
                    t.SendMessage(new Packet() { type = PacketType.StartCalc, id = infoMe.id });
                    count++;
                    UpdProgress(count, countAll, name);
                }
                UploadTimeEnd = DateTime.Now;
                CalculateTimeStart = DateTime.Now;
                RunExe();
                count++;
                UpdProgress(count, countAll, "Вычисления запущены");
            });
            calcThread.IsBackground = true;
            calcThread.Start();
        }
        #endregion

        #region Запуск и обрыв вычислений
        internal void StartCalculate(bool notDeleteFiles) // запуск вычислений
        {
            AllTimeStart = DateTime.Now;
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
            if (Process.GetProcessesByName(mainProc.ProcessName).Length != 0) // если процесс не завершился
            {
                mainProc.Kill();
            }
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
        #endregion

        #region Работа с инициатором в качестве исполнителя
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
                        CalculateTimeEnd = AllTimeEnd = DateTime.Now;
                        Log.ShowMessage("Общее время вычислений: " + (AllTimeEnd-AllTimeStart).TotalSeconds + " сек\nВремя на передачу файлов: " + (UploadTimeEnd-UploadTimeStart).TotalSeconds + " сек\nВремя рассчетов: " + (CalculateTimeEnd-CalculateTimeStart).TotalSeconds + " сек");
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
        #endregion

        #region Работа с пакетами
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
                            CalculateTimeEnd=AllTimeEnd = DateTime.Now;
                            isCalculate = false;
                            UpdProgress(2,2, "Вычисления завершены");
                            RefreshView();
                            Log.ShowMessage("Общее время вычислений: " + (AllTimeEnd - AllTimeStart).TotalSeconds + " сек\nВремя на передачу файлов: " + (UploadTimeEnd - UploadTimeStart).TotalSeconds + " сек\nВремя рассчетов: " + (CalculateTimeEnd - CalculateTimeStart).TotalSeconds + " сек");
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
        #endregion
    }
}
