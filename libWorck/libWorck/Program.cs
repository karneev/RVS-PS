using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace libWorck
{
    //основной мастер поток
    class Work
    {
        //прием.
        public class Connect // вспомогательный класс. Контейнер для желающих подключиться и передать информацию
        {        
            Thread LoadThread;
            int I, type;// номер машины и тип соединения
            Object data;// загружанные данные
            EventWaitHandle ew;
            NetworkStream s;// сокет через который было подключение
            bool ready = false;// ключь готовности принятия
            public Connect(NetworkStream s, int type, int I)
            {
                this.s = s;
                this.type = type;
                this.I = I;
                this.data = null;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                LoadThread=new Thread(run);
                LoadThread.Start();
            }
            private void run()
            {
                BinaryFormatter b = new BinaryFormatter();
                data=b.Deserialize(s);
                ready = true;
                ew.Set();
                s.Close();
                ew.Close();
            }
            public int getType()
            {
                return type;
            }
            public int getI()
            {
                return I;
            }
            public Object getData()
            {
                if (ready) return data;
                else return null;
            }
            public bool isReady()
            {
                return ready;
            }
            public void block()
            {
                if (!ready) ew.WaitOne();
            }
        }
        public class Recipient // контейнер для потоков обрабатывающих принятие данных
        {   
            Connect c = null;// контейнер сокета полдключившейся машины
            int I, type;// номер машины и тип соединения
            EventWaitHandle ew;
            public Recipient(int I, int type)
            {
                this.I = I;
                this.type = type;
                ew = new EventWaitHandle(false,EventResetMode.ManualReset);
                ew.Reset();
            }
            public void setC(Connect c)
            {
                this.c = c;
                ew.Set();
                ew.Close();
            }
            public int getI()
            {
                return I;
            }
            public int getType()
            {
                return type;
            }
            public Object getData()
            {
                if (c == null) return null;
                if (c.isReady()) return c.getData();
                else return null;
            }
            public bool isReady()
            {
                if (c != null) return c.isReady();
                else return false;
            }
            public void block()
            {
                if(c==null)ew.WaitOne();
                c.block();
            }
        }
        class Spindle // контейнер для соедитнения соотвецтвующих сокетов и потоков обработки принятия данных
        {
            List<Connect> In;
            List<Recipient> Out;
            public Spindle()
            {
                In = new List<Connect>();
                Out = new List<Recipient>();
            }
            public int get(Connect c)
            {
                if (Out.Count == 0)
                {
                    In.Add(c);
                    return 1;
                }
                int dd = -1;
                for (int i = 0; i < Out.Count; i++)
                {
                    if (c.getI() == Out[i].getI() && c.getType() == Out[i].getType())
                    {
                        dd = i;
                        break;
                    }
                }
                if (dd == -1)
                {
                    In.Add(c);
                    return 1;
                }
                Recipient r = Out[dd];
                Out.RemoveAt(dd);
                r.setC(c);
                return 0;
            }
            public int get(Recipient r)
            {
                if (In.Count == 0)
                {
                    Out.Add(r);
                    return 1;
                }
                int dd = -1;
                for (int i = 0; i < In.Count; i++)
                {
                    if (r.getType() == In[i].getType() && r.getI() == In[i].getI())
                    {
                        dd = i;
                        break;
                    }
                }
                if (dd == -1)
                {
                    Out.Add(r);
                    return 1;
                }
                Connect c = In[dd];
                In.RemoveAt(dd);
                r.setC(c);
                return 0;
            }
        }
        //синхронный условный переход
        abstract class SUP
        {
            protected class CUP
            {
                bool b;
                int I;
                public CUP(bool b, int I)
                {
                    this.b = b;
                    this.I = I;
                }
                public int getI()
                {
                    return I;
                }
                public bool getb()
                {
                    return b;
                }
            }
            protected int col;
            protected bool w;
            protected EventWaitHandle ew;
            public abstract void add(int I, bool b);
            public abstract bool collect(bool self, int I);
            public abstract void sender(bool self, int I);
        }
        class SUPAll:SUP
        {
            List<CUP> z;
            Hashtable r;
            List<IPEndPoint> Iplist;
            public SUPAll(int col,List<IPEndPoint> Iplist)
            {
                this.z = new List<CUP>();
                this.r = new Hashtable(col);
                this.Iplist=Iplist;
                this.col = col;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                w = false;
            }
            public override void sender(bool self, int I)
            {
                for (int i = 0; i < Iplist.Count; i++)
                {
                    if (i != I)
                    {
                        TcpClient c = new TcpClient();
                        c.Connect(Iplist[i]);
                        NetworkStream s = c.GetStream();
                        byte[] buffer = new byte[255];
                        if (s.ReadByte() == 1) continue;
                        buffer[0] = 2;
                        buffer[1] = 1;
                        buffer[2] =(self)?(byte)1:(byte)0;
                        s.Write(buffer, 0, 255);
                        s.Close();
                    }
                }
            }
            public override void add(int I, bool b)
            {
                CUP a = new CUP(b, I);
                lock (z)
                {
                    if (r.ContainsKey(a.getI()))
                    {
                        z.Add(a);
                    }
                    else
                    {
                        r.Add(a.getI(), a.getb());
                        if (w && col - 1 == r.Count)
                        {
                            ew.Set();
                        }
                    }
                }
            }
            public override bool collect(bool self, int I)
            {
                Monitor.Enter(z);
                w = true;
                if (r.Count != col - 1)
                {
                    Monitor.Exit(z);
                    ew.WaitOne();
                    Monitor.Enter(z);
                }
                bool rbool = true;
                for (int i = 0; i < col; i++)
                {
                    if (I == i) rbool = rbool && self;
                    else rbool = rbool && (bool)r[i];
                }
                r.Clear();
                for (int i = 0; i < z.Count; i++)
                {
                    if (!r.ContainsKey(z[i].getI()))
                    {
                        r.Add(z[i].getI(), z[i].getb());
                        z.RemoveAt(i);
                        i--;
                    }
                }
                w = false;
                ew.Reset();
                Monitor.Exit(z);
                return rbool;
            }
            ~SUPAll()
            {
                ew.Close();
            }
        }
        class SUPOneCenter:SUP
        {
            List<CUP> z;
            Hashtable r;
            List<IPEndPoint> Iplist;
            public SUPOneCenter(int col, List<IPEndPoint> Iplist)
            {
                this.z = new List<CUP>();
                this.r = new Hashtable(col);
                this.col = col;
                this.Iplist = Iplist;
                this.ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                this.w = false;
                this.ew.Reset();
            }
            public override void sender(bool self, int I)
            {
            }
            public override void add(int I, bool b)
            {
                CUP a = new CUP(b, I);
                lock (z)
                {
                    if (r.ContainsKey(a.getI()))
                    {
                        z.Add(a);
                    }
                    else
                    {
                        r.Add(a.getI(), a.getb());
                        if (w && col - 1 == r.Count)
                        {
                            ew.Set();
                        }
                    }
                }
            }
            public override bool collect(bool self, int I)
            {
                Monitor.Enter(z);
                w = true;
                if (r.Count != col - 1)
                {
                    Monitor.Exit(z);
                    ew.WaitOne();
                    Monitor.Enter(z);
                }
                bool rbool = true;
                for (int i = 0; i < col; i++)
                {
                    if (I == i) rbool = rbool && self;
                    else rbool = rbool && (bool)r[i];
                }
                r.Clear();
                for (int i = 0; i < z.Count; i++)
                {
                    if (!r.ContainsKey(z[i].getI()))
                    {
                        r.Add(z[i].getI(), z[i].getb());
                        z.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < col; i++)
                {
                    if (i != I)
                    {
                        TcpClient c = new TcpClient();
                        c.Connect(Iplist[i]);
                        NetworkStream s = c.GetStream();
                        byte[] buffer = new byte[255];
                        if (s.ReadByte() == 1) continue;
                        buffer[0] = 2;
                        buffer[1] = 1;
                        buffer[2] = (rbool) ? (byte)1 : (byte)0;
                        s.Write(buffer, 0, 255);
                        s.Close();
                    }
                }
                w = false;
                ew.Reset();
                Monitor.Exit(z);
                return rbool;
            }
            ~SUPOneCenter()
            {
                ew.Close();
            }
        }
        class SUPOneLeft:SUP
        {
            IPEndPoint res;
            bool rezalt,ready;
            Object Block;
            public SUPOneLeft(IPEndPoint res)
            {
                this.res = res;
                this.col = 0;
                w = false;
                Block = new Object();
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
            }
            public override void sender(bool self, int I)
            {
                TcpClient c = new TcpClient();
                c.Connect(res);
                NetworkStream s = c.GetStream();
                byte[] buffer = new byte[255];
                if (s.ReadByte() == 1) return;
                buffer[0] = 2;
                buffer[1] = 1;
                buffer[2] = (self) ? (byte)1 : (byte)0;
                s.Write(buffer, 0, 255);
                s.Close();
            }
            public override void add(int I, bool b)
            {
                lock (Block)
                {
                    rezalt = b;
                    if (w)
                    {
                        ew.Set();
                    }
                }
            }
            public override bool collect(bool self, int I)
            {
                Monitor.Enter(Block);
                w = true;
                Monitor.Exit(Block);
                ew.WaitOne();
                ew.Reset();
                return rezalt;
            }
        }
        //безусловный переход
        class GJ//пререписать на припем функций
        {
            Hashtable DF;
            public GJ()
            {
                DF = new Hashtable();
            }
            public void set(int i, delfunc d)
            {
                if (DF.ContainsKey(i)) DF.Remove(i);
                DF.Add(i, d);
            }
            public delfunc get(int i)
            {
                if (!DF.ContainsKey(i)) return null;
                else return (delfunc)DF[i];
            }
        }
        //дискрипторы передачи данных
        public class DiscretSendData
        {
            IPEndPoint Ip;
            Object data;
            bool ready;
            EventWaitHandle ew;
            public DiscretSendData(IPEndPoint Ip, Object data)
            {
                this.Ip = Ip;
                this.data = data;
                this.ready = false;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                (new Thread(send)).Start();
            }
            void send()
            {
                byte[] buffer = new byte[255];
                TcpClient c = new TcpClient();
                //c.SendBufferSize = 2 * c.SendBufferSize;
                c.Connect(Ip);
                NetworkStream s =c.GetStream();
                if (s.ReadByte() == 1) return;
                BinaryFormatter bf = new BinaryFormatter();
                buffer[0] = 1;
                buffer[1] = 1;
                s.Write(buffer,0,255);
                bf.Serialize(s, data);
                ready = true;
                ew.Set();
                s.Close();
                ew.Close();
            }
            public bool isReady()
            {
                return ready;
            }
            public void block()
            {
                if(!ready)ew.WaitOne();
            }
        }
        public class CollectSendData
        {
            IPEndPoint Ip;
            Object data;
            bool ready;
            EventWaitHandle ew;
            public CollectSendData(IPEndPoint Ip, Object data)
            {
                this.Ip = Ip;
                this.data = data;
                this.ready = false;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                (new Thread(send)).Start();
            }
            void send()
            {

                byte[] buffer = new byte[255];
                TcpClient c = new TcpClient();
                c.Connect(Ip);
                NetworkStream s = c.GetStream();
                if (s.ReadByte() == 1) return;
                BinaryFormatter bf = new BinaryFormatter();
                buffer[0] = 1;
                buffer[1] = 3;
                s.Write(buffer, 0, 255);
                bf.Serialize(s, data);
                ready = true;
                ew.Set();
                s.Close();
                ew.Close();
            }
            public bool isReady()
            {
                return ready;
            }
            public void block()
            {
                if(!ready)ew.WaitOne();
            }
        }
        public class TrunsSendData
        {
            List<IPEndPoint> Ip;
            int I;
            Object data;
            int count;
            EventWaitHandle ew;
            public TrunsSendData(List<IPEndPoint> Ip, int I, Object data)
            {
                this.Ip = Ip;
                this.I = I;
                this.data = data;
                this.count = Ip.Count - 1;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                for (int i = 0; i < Ip.Count; i++)
                {
                    if (i != I) (new Thread(send)).Start(i);
                }
            }
            void send(object ii)
            {
                int i = (int)ii;
                byte[] buffer = new byte[255];
                TcpClient c = new TcpClient();
                c.Connect(Ip[i]);
                NetworkStream s = c.GetStream();
                if (s.ReadByte() == 1) return;
                BinaryFormatter bf = new BinaryFormatter();
                buffer[0] = 1;
                buffer[1] = 2;
                s.Write(buffer,0,255);
                bf.Serialize(s, data);
                lock (data)
                {
                    count--;
                    if(count==0)
                    {
                        ew.Set();
                        ew.Close();
                    }
                }
                s.Close();
            }
            public bool isReady()
            {
                return count == 0;
            }
            public void block()
            {
                if (!(count==0)) ew.WaitOne();
            }
        }
        public class CollectorRecipientData
        {
            Recipient[] data;
            int I;
            public CollectorRecipientData(Recipient[] data,int I)
            {
                this.data = data;
                this.I = I;
            }
            public bool isReady()
            {
                bool t=true;
                for (int i = 0; i < data.Length; i++)
                {
                    if (I != i) t = t && data[i].isReady();
                }
                return t;
            }
            public Object getData(int i)
            {
                if (i == I) return null;
                if (i < 0 && i >= data.Length) return null;
                return data[i].getData();
            }
            public void block()
            {
                for (int i = 0; i < data.Length; i++)
                    if (i != I) data[i].block();
            }

        }
        public class TrunsRecipientData//прием сразу.
        {
            Recipient r;
            public TrunsRecipientData(Recipient r)
            {
                this.r = r;
            }
            public bool isReady()
            {
                return r.isReady();
            }
            public Object getData()
            {
                return r.getData();
            }
            public int getI()
            {
                return r.getI();
            }
            public int getType()
            {
                return r.getType();
            }
            public void block()
            {
                r.block();
            }
        }
        public class DiscretRecipientData//прием сразу.
        {
            Recipient r;
            public DiscretRecipientData(Recipient r)
            {
                this.r = r;
            }
            public bool isReady()
            {
                return r.isReady();
            }
            public Object getData()
            {
                return r.getData();
            }
            public int getI()
            {
                return r.getI();
            }
            public int getType()
            {
                return r.getType();
            }
            public void block()
            {
                r.block();
            }
        }
        public delegate void delfunc(byte[] data);
        Thread master,slave; //потоки обрабатывающий события и отыгрывающий пользовательскую программу.
        List<IPEndPoint> Iplist;// список конечных точек серверов других машин
        int I,port,breakF;// номер своего IP адреса, номер прослушающего порта, номер исключающей функции. 
        byte[] breakData;
        TcpListener s;// серверный сокет
        Spindle sp;// обработка ипоступающих запросов на принятие 
        SUP sup;//обработка снхронизированного обобщеного условного перехода
        GJ gj;//хранилише делегатов функций 
        bool [] anB;//массив для ассинхронного условногог перехода.
        void masterFun()//функция поддержки
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
                    TcpClient h = s.AcceptTcpClient();
                    //h.ReceiveBufferSize = 2 * h.ReceiveBufferSize;
                    NetworkStream stream=h.GetStream();
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
                                lock (sp)
                                {
                                    sp.get(new Connect(stream, bytes[1], bb));
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
                                        stream.Read(bytes,0,sizeof(Int32));
                                        int y = BitConverter.ToInt32(bytes, 0);
                                        if(y!=0)
                                        {
                                            byte[] breakData = new byte[y];
                                            stream.Read(breakData,0,y);
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
            }
        }
        public Work()
        {
            //FileInfo f = new FileInfo("Index");
            this.Iplist = new List<IPEndPoint>();
            //byte [] buffer= new byte[4];
            StreamReader R = new StreamReader("Index.txt");
            //FileStream R = f.OpenRead();
            /*R.Read(buffer,0,4);
            int y = BitConverter.ToInt32(buffer,0);*/
            int y = Convert.ToInt32(R.ReadLine());
            /*R.Read(buffer,0,4);
            this.I= BitConverter.ToInt32(buffer,0);*/
            this.I = Convert.ToInt32(R.ReadLine());
            for (int i = 0; i < y; i++)
            {
                /*
                R.Read(buffer,0,4);
                int p= BitConverter.ToInt32(buffer,0);
                R.Read(buffer,0,4);
                Iplist.Add(new IPEndPoint(new IPAddress(buffer),p));*/
                Iplist.Add(new IPEndPoint(IPAddress.Parse(R.ReadLine()), 8002));
            }
            R.Close();
            this.sp = new Spindle();
            this.anB = new bool[256];
            for (int i = 0; i < anB.Length; anB[i] = false, i++) ;
            this.port = 8002;
            this.breakF = -1;
            this.breakData = null;
            this.sup = null; 
            this.gj = new GJ();
            IPEndPoint ePoint = new IPEndPoint(0, port);
            this.s = new TcpListener(ePoint);
            this.s.Start();
        }
        public void setSUP(int i)
        {
            if (i < 0 && i >= Iplist.Count) sup = new SUPAll(Iplist.Count,Iplist);
            else if (i == I) this.sup = new SUPOneCenter(Iplist.Count, Iplist);
            else this.sup = new SUPOneLeft(Iplist[i]);
        }
        public void start()// нах надо?
        {
            if (sup == null) sup = new SUPAll(Iplist.Count, Iplist);
            Thread.Sleep(20000);
            master = new Thread(masterFun);
            master.IsBackground = true;
            master.Start();
            SGCJ(true);
            slaveFun();
            SGCJ(true);
        }
        public DiscretSendData Send(int i,Object data)
        {
            if(i<0||i>Iplist.Count)return null;
            DiscretSendData d = new DiscretSendData(Iplist[i], data);
            return d;
        }
        public TrunsSendData TrunsSend(Object data)
        {
            TrunsSendData t = new TrunsSendData(Iplist, I, data);
            return t;
        }
        public DiscretRecipientData GetData(int I)
        {
            Recipient r = new Recipient(I, 1);
            DiscretRecipientData rd = new DiscretRecipientData(r);
            lock (sp)
            {
                sp.get(r);
            }
            return rd;
        }
        public TrunsRecipientData TransGetData(int I)
        {
            Recipient r = new Recipient(I, 2);
            TrunsRecipientData rd = new TrunsRecipientData(r);
            lock (sp)
            {
                sp.get(r);
            }
            return rd;
        }
        public CollectorRecipientData getAllData()
        {
            Recipient [] data = new Recipient[Iplist.Count];
            for (int i = 0; i < data.Length; i++)
            {
                if (I != i)
                {
                    data[i] = new Recipient(i, 3);
                    lock (sp)
                    {
                        sp.get(data[i]);
                    }
                }
                else data[i] = null;
            }
            CollectorRecipientData rd = new CollectorRecipientData(data, I);
            return rd;
        }
        public CollectSendData Collectsend(int i, Object data)
        {
            if(i<0||i>Iplist.Count)return null;
            if (i == I) return null;
            CollectSendData t = new CollectSendData(Iplist[i], data);
            return t;
        }
        public bool AGCJ(byte a)//асинхронный обобщенный условный переход аргумент номер буфера
        {
            if (a < 0 || a > 255) return false;
            bool rezalt=true;
            for (int i = 0; i < Iplist.Count; i++)
            {
                if (i != I)
                {
                    TcpClient c = new TcpClient(Iplist[i]);
                    NetworkStream s = c.GetStream();
                    byte[] buffer = new byte[255];
                    if (s.ReadByte()== 1) return false;
                    buffer[0] = 2;
                    buffer[1] = 0;
                    buffer[2] = a;
                    s.Write(buffer,0,255);
                    int r=s.ReadByte();
                    s.Close();
                    rezalt = rezalt && (r != 0);
                }
                else
                {
                    rezalt = rezalt && anB[a];
                }
            }
            return rezalt;
        }
        public bool SGCJ(bool b)//синхронный обобщенный условный переход
        {
            sup.sender(b, I);
            return sup.collect(b, I);
        }
        public bool INT(int i, byte j,byte[] m)// запуск перывания на i машине j функции с m данными
        {
            TcpClient c = new TcpClient();
            c.Connect(Iplist[i]);
            NetworkStream s = c.GetStream();
            if (s.ReadByte() == 1) return false;
            byte[] buffer = new byte[255];
            buffer[0] = 3;
            buffer[1] = j;
            s.Write(buffer,0,255);
            if(s.ReadByte()==1)return false;
            if(m==null)s.Write(BitConverter.GetBytes(0),0,sizeof(Int32));
            else
            { 
                s.Write(BitConverter.GetBytes(m.Length),0,sizeof(Int32));
                s.Write(m,0,m.Length);
            }
            s.Close();
            return true;
        }
        public void setFunction(byte i, delfunc df)// установка на место i функции df функции 
        {
            lock (gj)
            {
                gj.set(i, df);
            }
        }
        public void BreakPoint()// выполнение прерывания
        {
            lock (gj)
            {
                if (breakF != -1) 
                    if (gj.get(breakF) != null)
                    {
                        gj.get(breakF)(breakData);
                        breakF = -1;
                        breakData = null;
                    }
            }
        }
        public void setKey(byte a, bool f)//установка a переменной в f значение 
        {
            lock (anB)
            {
                anB[a] = f;
            }
        }
        public int getI()
        {
            return I;
        }
        public int getCount()
        {
            return Iplist.Count();
        }
        public virtual void slaveFun()
        {
        }//функция для перезаписи.
    }
    class mainFrame : Work
    {
        public override void slaveFun()
        {
            double checker = 0;
            DateTime check;
            StreamReader R = new StreamReader("Work.txt");
            double er = (double)Convert.ToDouble(R.ReadLine());
            int N = Convert.ToInt32(R.ReadLine());
            double[][] A = new double[N/getCount()][];
            int ic = 0;
            for (;ic < (N / getCount()) * getI(); R.ReadLine(), ic++);
            for (int i=0;ic < (N / getCount()) * (getI()+1) && ic<N;i++,ic++)
            {
                A[i] = R.ReadLine().Split(new char[] { ' ' }).Select(Double.Parse).ToArray();
            }
            for (; ic < N; R.ReadLine(), ic++) ;
            double[] F = R.ReadLine().Split(new char[]{' '}).Skip(getI()*N/getCount()).Take(N/getCount()).Select(Double.Parse).ToArray();
            double[] X = new double[F.Length];
            double[] timeX = new double[F.Length];
            double[] buffer;
            bool end = true;
            int JJJ = N / getCount();
            int JJJ1 = getI() * JJJ;

            Console.WriteLine("Start");
            DateTime time = System.DateTime.Now;
            int it=0;
            int pred = (getI() == 0) ? getCount() - 1 : getI() - 1, next = (getI() == getCount() - 1) ? 0 : getI() + 1;
            int iterr = 0;
            do{
                //check = DateTime.Now;
                for(int i=0;i<F.Length;i++)
                {
                    timeX[i] = F[i];
                }
                buffer = X;
                int w=getI();
                //checker += (System.DateTime.Now - check).TotalMilliseconds;
                for (int m = 0; m < getCount(); m++)
                {
                    int JJJ2 = w * JJJ;
                    Work.DiscretRecipientData R1 = GetData(pred);
                    Work.DiscretSendData R2 = Send(next, buffer);
                    check = System.DateTime.Now;
                    for (int i =0; i < timeX.Length; i++)
                    {
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            iterr++;
                            if((i+JJJ1)!=(j+JJJ2))timeX[i] -= A[i][(JJJ2)+j] * buffer[j];
                        }
                    }
                    w=(w+1==getCount())?0:w+1;
                    checker += (System.DateTime.Now - check).TotalMilliseconds;
                    R1.block();

                    buffer =(double[])R1.getData();// Thread.Sleep(5000);

                }
                end = true;
                // check = DateTime.Now;
                for (int i =0; i < JJJ; i++)
                {
                    timeX[i] /= A[i][i + JJJ * getI()];
                    end = end && (er > Math.Abs(X[i] - timeX[i]));
                    X[i] = timeX[i];
                }
                // checker += (System.DateTime.Now - check).TotalMilliseconds;
                it++; 
            }while (!SGCJ(end));
            DateTime time1 = System.DateTime.Now;
            for (int i = 0; i < JJJ; i++)
            {
                Console.WriteLine("X{0:D}:{1:E}",getI()*JJJ+i,X[i]);
            }
            Console.Write("Time work: ");
            Console.WriteLine((time1-time).TotalMilliseconds);
            Console.WriteLine(checker);
            Console.WriteLine(iterr);
            Console.ReadLine();
            
        }
    }
    class Program
    {
        public static void Main(string[] args)
        {
            
            mainFrame m = new mainFrame();
            m.setSUP(0);
            m.start();

        }
    }
}

