using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace Library
{
    /// <summary>
    /// Базовый класс библиотеки
    /// </summary>
    public partial class Work : ILoggable
    {
        public void log(string message)
        {
            StreamWriter sw = File.AppendText("LibraryLog.txt");
            sw.WriteLine(DateTime.Now.ToString() + " " + message);
            sw.Close();
        }
        /// <summary>
        /// Делегат для функции прерывания
        /// </summary>
        /// <param name="data"> Список аргументов </param>
        public delegate void delfunc(byte[] data);
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public Work(int prt = 8002)
        {
            this.port = prt;
            Iplist = new List<IPEndPoint>();
            if (File.Exists("iplist.txt"))
            {
                List<string> myIP = new List<string>();
                StreamReader R = new StreamReader("iplist.txt");
                int count = Convert.ToInt32(R.ReadLine());
                // Получение имени компьютера.
                string host = Dns.GetHostName();
                // Получение ip-адреса.
                IPHostEntry iphostentry = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // если адрес ipv4
                    {
                        myIP.Add(ipaddress.ToString());
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    string s = R.ReadLine();
                    if (myIP.Contains(s) || s.Equals("127.0.0.1")) currentIndex = i;
                    Iplist.Add(new IPEndPoint(IPAddress.Parse(s), this.port));
                }
                R.Close();
            }
            else
            {
                currentIndex = 0;
                Iplist.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port));
            }
            mainSpindle = new Spindle();
            anB = new bool[256];
            for (int i = 0; i < anB.Length; anB[i] = false, i++) ;
            breakF = -1;
            breakData = null;
            sup = null;
            gj = new GJ();
            IPEndPoint ePoint = new IPEndPoint(0, port);
            mainSocket = new TcpListener(ePoint);
            mainSocket.Start();
        }

        /// <summary>
        /// Установление модуля обобщенного условного перехода
        /// </summary>
        /// <param name="i"> Номер типа перехода </param>
        public void setSUP(int i)
        {
            if (i < 0 || i >= Iplist.Count)
                sup = new SUPAll(Iplist.Count, Iplist);
            else if (i == currentIndex)
                sup = new SUPOneCenter(Iplist.Count, Iplist);
            else
                sup = new SUPOneLeft(Iplist[i]);
        }

        /// <summary>
        /// Запуск
        /// </summary>
        public void start()
        {
            if (sup == null) sup = new SUPAll(Iplist.Count, Iplist);
            Thread.Sleep(500);
            master = new Thread(masterFun);
            master.IsBackground = true;
            master.Start();
            SGCJ(true);
            slaveFun();
            SGCJ(true);
        }
        #region Дифференцированная схема обмена

        /* 

            Первой предоставленной схемой обмена из способов является дифференцированная схема обмена.
        Она требуется тогда когда нужно передать данные из одной машины в другую. 
        Для нее существуют команда отправки Send(int,Object), для отправляющей стороны, 
        и команда приема GetData(int I), для принимающей стороны. 

            Функция Send(int,Object) требует номер машины, 
        на которую передаются данные, и объект, который должен быть передан. 
        Она возвращает объект-дескриптор данной передачи класса DiscretSendData.
        Данный объект предоставляет информацию об отправке данных. 
        С помощью метода данного класса isReady() можно получить признак, определяющий окончание передачи, 
        а с помощью метода block() можно остановить процесс вычисления до окончания передачи данных. 

           Функция GetData (int) требует номер машины, с которой передаются данные. 
        Она возвращает объект-дескриптор данной передачи класса DiscretRecipientData. 
        Данный объект предоставляет информацию о приеме данных. 
        С помощью метода данного класса isReady() можно получить признак, определяющий окончание передачи, 
        а с помощью метода block() можно остановить процесс вычисления  до окончания приема данных. 

            Также у данного класса есть метод getData(), 
        который возвращает переданные данные в виде объекта класса Object. 
        Пользователю достаточно представить этот объект в нужном ему типе. 

        */
        /// <summary>
        /// Отправка данных по дифференцированной схеме обмена
        /// </summary>
        /// <param name="i"> Номер машины </param>
        /// <param name="data"> Данные для передачи </param>
        /// <returns></returns>
        public DiscretSendData Send(int i, object data)
        {
            if (i < 0 || i > Iplist.Count)
                return null;
#if DEBUG
            log("Discret send data to " + i + " start");
#endif
            DiscretSendData d = new DiscretSendData(Iplist[i], data);
#if DEBUG
            log("Discret send data to " + i + " stop");
#endif
            return d;
        }
        /// <summary>
        /// Получение данных по дифференцированной схеме обмена
        /// </summary>
        /// <param name="I"> Номер машины </param>
        /// <returns></returns>
        public DiscretRecipientData GetData(int I)
        {
#if DEBUG
            log("Discret receive data from " + I + " start");
#endif
            Recipient r = new Recipient(I, 1);
            DiscretRecipientData rd = new DiscretRecipientData(r);
            lock (mainSpindle)
            {
                mainSpindle.Get(r);
            }
#if DEBUG
            log("Discret receive data from " + I + " stop");
#endif
            return rd;
        }
        #endregion
        #region Трансляционная схема обмена

        /*

            Второй предоставленной схемой обмена является трансляционная схема обмена. 
        Она требуется тогда, когда нужно передать данные из одной машины на все другие.
        Для нее существует команда отправки TrunsSend(Object data) для отправляющей стороны, 
        и команда приема TransGetData(int I) для принимающей стороны. 

            Функция TrunsSend(Object data) требует объект, который должен быть передан на все другие машины. 
        Она возвращает объект-дескриптор данной передачи класса TrunsSendData. 
        Данный объект предоставляет информацию об отправке данных. 
        С помощью метода данного класса isReady() можно получить признак, 
        определяющий окончание всех передач, а с помощью метода block() можно остановить процесс вычисления  
        до окончания всех передач данных. 

            Функция TransGetData(int I) требует номер машины, с которой трансляционно передаются данные. 
        Она возвращает объект-дескриптор данной передачи класса TrunsRecipientData.
        Данный объект предоставляет информацию о приеме данных. 
        С помощью метода данного класса isReady() можно получить признак, определяющий окончание передачи, 
        а с помощью метода block() можно остановить процесс вычисления до окончания приема данных 
        на данной машине. 

            Также у данного класса есть метод getData(), который возвращает переданные данные 
        в виде объекта класса Object. Пользователю достаточно представить этот объект в нужном ему типе. 

        */
        /// <summary>
        /// Отправка данных по трансляционной схеме обмена
        /// </summary>
        /// <param name="data"> Данные для отправки </param>
        /// <returns></returns>
        public TrunsSendData TrunsSend(object data)
        {
#if DEBUG
            log("Truns send data start");
#endif
            TrunsSendData t = new TrunsSendData(Iplist, currentIndex, data);
#if DEBUG
            log("Truns send data stop");
#endif
            return t;
        }
        /// <summary>
        /// Получение данных по трансляционной схеме обмена
        /// </summary>
        /// <param name="I"> Номер машины </param>
        /// <returns></returns>
        public TrunsRecipientData TransGetData(int I)
        {
#if DEBUG
            log("Truns receive data from " + I + " start");
#endif
            Recipient r = new Recipient(I, 2);
            TrunsRecipientData rd = new TrunsRecipientData(r);
            lock (mainSpindle)
            {
                mainSpindle.Get(r);
            }
#if DEBUG
            log("Truns receive data from " + I + " stop");
#endif
            return rd;
        }

        #endregion
        #region Коллекторная схема обмена

        /*

          Третьей предоставленной схемой обмена является коллекторная схема обмена. 
        Она требуется тогда, когда нужно передать данные из всех машин в одну. 
        Для нее существуют команда отправки Collectsend(int, Object), для отправляющей стороны, 
        и команда приема getAllData(), для принимающей стороны. 

           Функция Collectsend(int, Object) требует номер машины-коллектора и объект, 
        который должен быть передан на всем другим машинам. 
        Она возвращает объект-дескриптор данной передачи класса CollectSendData. 
        Данный объект предоставляет информацию об отправке данных. 
        С помощью метода данного класса isReady() можно получить признак, определяющий окончание передачи, 
        а с помощью метода block() можно остановить процесс вычисления до окончания передачи данных. 

           Функция getAllData() возвращает объект-дескриптор данной передачи класса CollectorRecipientData. 
        Данный объект предоставляет информацию о приеме данных. 
        С помощью метода данного класса isReady() можно получить признак, 
        определяющий окончание всех передач, а с помощью метода block() можно остановить процесс вычисления 
        до окончания приема всех данных. 

           Также у данного класса есть метод getData(int), который возвращает переданные данные 
        с выбранной машины в виде объекта класса Object. 
        Пользователю достаточно представить этот объект в нужном ему типе.

        */

        /// <summary>
        /// Отправка данных по коллекторной схеме обмена
        /// </summary>
        /// <param name="i"> Номер машины </param>
        /// <param name="data"> Данные </param>
        /// <returns></returns>
        public CollectSendData CollectorSend(int i, object data)
        {
            if (i < 0 || i > Iplist.Count) return null;
            if (i == currentIndex) return null;
#if DEBUG
            log("Collect send data to " + i + " start");
#endif
            CollectSendData t = new CollectSendData(Iplist[i], data);
#if DEBUG
            log("Collect send data to " + i + " stop");
#endif
            return t;
        }

        /// <summary>
        /// Прием данных по коллекторной схеме обмена
        /// </summary>
        /// <returns></returns>
        public CollectorRecipientData getAllData()
        {
            Recipient[] data = new Recipient[Iplist.Count];
            for (int i = 0; i < data.Length; i++)
            {
                if (currentIndex != i)
                {
#if DEBUG
                    log("Collect receive data from " + i + " start");
#endif
                    data[i] = new Recipient(i, 3);
                    lock (mainSpindle)
                    {
                        mainSpindle.Get(data[i]);
                    }
#if DEBUG
                    log("Collect receive data from " + i + " stop");
#endif
                }
                else data[i] = null;
            }
            CollectorRecipientData rd = new CollectorRecipientData(data, currentIndex);
            return rd;
        }

        #endregion
        #region Обобщенный условный переход

        /// <summary>
        /// Синхронный обобщенный условный переход
        /// </summary>
        /// <param name="b"> Признак данной машины </param>
        /// <returns> Обобщенный признак </returns>
        public bool SGCJ(bool b)
        {
            sup.sender(b, currentIndex);
            return sup.collect(b, currentIndex);
        }

        #endregion

        #region Асинхронный сбор признака

        /// <summary>
        /// Установка признака
        /// </summary>
        /// <param name="a"> Имя признака </param>
        /// <param name="f"> Его значение </param>
        public void setKey(byte a, bool f)
        {
            lock (anB)
            {
                anB[a] = f;
            }
        }

        /// <summary>
        /// Сбор признака
        /// </summary>
        /// <param name="a"> Имя признака </param>
        /// <returns> Обобщенный признак признаков других машин </returns>
        public bool AGCJ(byte a)
        {
            bool result = true;
            for (int i = 0; i < Iplist.Count; i++)
            {
                if (i != currentIndex)
                {
                    TcpClient c = new TcpClient(Iplist[i]);
                    NetworkStream s = c.GetStream();
                    byte[] buffer = new byte[255];
                    if (s.ReadByte() == 1) return false;
                    buffer[0] = 2;
                    buffer[1] = 0;
                    buffer[2] = a;
                    s.Write(buffer, 0, 255);
                    int r = s.ReadByte();
                    s.Close();
                    result = result && (r != 0);
                    c.Close();
                }
                else
                {
                    result = result && anB[a];
                }
            }
            return result;
        }

        #endregion

        #region Обобщенный безусловный переход (ОБП)

        /// <summary>
        /// Инициирует ОБП
        /// </summary>
        /// <param name="i"> Номер машины на которой будет запускаться ОБП </param>
        /// <param name="j"> Номер запускаемой функции </param>
        /// <param name="m"> Передаваемые ей аргументы </param>
        /// <returns></returns>
        public bool INT(int i, byte j, byte[] m)
        {
            TcpClient c = new TcpClient();
            c.Connect(Iplist[i]);
            NetworkStream s = c.GetStream();
            if (s.ReadByte() == 1) return false;
            byte[] buffer = new byte[255];
            buffer[0] = 3;
            buffer[1] = j;
            s.Write(buffer, 0, 255);
            if (s.ReadByte() == 1) return false;
            if (m == null) s.Write(BitConverter.GetBytes(0), 0, sizeof(Int32));
            else
            {
                s.Write(BitConverter.GetBytes(m.Length), 0, sizeof(Int32));
                s.Write(m, 0, m.Length);
            }
            s.Close();
            c.Close();
            return true;
        }

        /// <summary>
        /// Установка на место i функции df
        /// </summary>
        /// <param name="i"> Номер функции </param>
        /// <param name="df"></param>
        public void setFunction(byte i, delfunc df)
        {
            lock (gj)
            {
                gj.Set(i, df);
            }
        }

        #endregion

        /// <summary>
        /// Выполнение прерывания
        /// </summary>
        public void BreakPoint()
        {
            lock (gj)
            {
                if (breakF != -1)
                    if (gj.Get(breakF) != null)
                    {
                        gj.Get(breakF)(breakData);
                        breakF = -1;
                        breakData = null;
                    }
            }
        }

        /// <summary>
        /// Номер машины
        /// </summary>
        /// <returns>Текущий индекс машины</returns>
        public int getIndex()
        {
            return currentIndex;
        }

        /// <summary>
        /// Количество машин в подсистеме
        /// </summary>
        /// <returns></returns>
        public int getCount()
        {
            return Iplist.Count();
        }

        /// <summary>
        /// Функция для определения самого алгоритма
        /// </summary>
        public virtual void slaveFun() { }
        /// <summary>
        /// Модуль приема сообщений
        /// </summary>
        void masterFun()
        {
            /*
             * 1 - передача(прием)
             *      1 - направленная
             *      2 - трансляционная 
             *      3 - колекторная 
             * 2 - условнй переход
             *      0 - не синхронизированный
             *      1 - синхронизированный
             * 3 - безусловный переход
             * 4 - синхронизация
             */

            byte[] bytes = new byte[255];
            try
            {
                while (true)
                {
                    TcpClient h = mainSocket.AcceptTcpClient();
                    //h.ReceiveBufferSize = 2 * h.ReceiveBufferSize;
                    NetworkStream stream = h.GetStream();
                    int bb = -1;
                    for (int i = 0; i < Iplist.Count; i++)
                    {
                        if ((h.Client.RemoteEndPoint as IPEndPoint).Address.Equals(Iplist[i].Address))
                        {
                            bb = i;
                            break;
                        }
                    }

                    if (bb == -1)
                    {
                        stream.WriteByte(1);
                        h.Close();
                        continue;
                    }
                    stream.WriteByte(0);
                    stream.Read(bytes, 0, 255);
                    switch (bytes[0])
                    {
                        case 1:
                            {
                                lock (mainSpindle)
                                {
                                    mainSpindle.Get(new Connect(stream, bytes[1], bb));
                                }
                                break;
                            }
                        case 2:
                            {
                                if (bytes[1] == 0)
                                {
                                    lock (anB)
                                    {
                                        if (anB[bytes[2]]) stream.WriteByte(1);
                                        else stream.WriteByte(0);
                                    }
                                    stream.Close();
                                }
                                else
                                {
                                    stream.Close();
                                    lock (sup)
                                    {
                                        sup.add(bb, (bytes[2] == 1) ? true : false);
                                    }
                                }
                                break;
                            }
                        case 3:
                            {
                                lock (gj)
                                {
                                    if (breakF != -1)
                                    {
                                        stream.WriteByte(1);
                                        stream.Close();
                                    }
                                    else
                                    {
                                        breakF = bytes[1];
                                        stream.WriteByte(0);
                                        stream.Read(bytes, 0, sizeof(Int32));
                                        int y = BitConverter.ToInt32(bytes, 0);
                                        if (y != 0)
                                        {
                                            byte[] breakData = new byte[y];
                                            stream.Read(breakData, 0, y);
                                        }
                                        stream.Close();
                                    }
                                }
                                break;
                            }
                    }

                }
            }
            catch (ThreadAbortException e)
            {
                log(e.Message);
            }
        }
        /// <summary>
        /// Потоки обрабатывающий события и отыгрывающий пользовательскую программу
        /// </summary>
        Thread master/*, slave*/;
        /// <summary>
        /// Список конечных точек серверов других машин
        /// </summary>
        List<IPEndPoint> Iplist;
        /// <summary>
        /// Номер своего IP адреса, номер прослушающего порта, номер исключающей функции
        /// </summary>
        int currentIndex, port, breakF;
        byte[] breakData;
        /// <summary>
        /// Cерверный сокет
        /// </summary>
        TcpListener mainSocket;
        /// <summary>
        /// Обработка поступающих запросов на принятие
        /// </summary>
        Spindle mainSpindle;
        /// <summary>
        /// Массив для ассинхронного условного перехода
        /// </summary>
        bool[] anB;
        SUP sup; //обработка снхронизированного обобщеного условного перехода
        GJ gj; //хранилише делегатов функций для ОБП
    }
}
