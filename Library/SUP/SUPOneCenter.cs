using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Collections;
namespace Library
{
    /// <summary>
    /// Способ реализации модуля обобщеного условного перехода "ВСЕ ОДНОМУ" для машины-сборщика
    /// </summary>
    class SUPOneCenter : SUP
    {
        List<CUP> z;
        Hashtable r;
        List<IPEndPoint> Iplist;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="Iplist"></param>
        public SUPOneCenter(int col, List<IPEndPoint> Iplist)
        {
            z = new List<CUP>();
            r = new Hashtable(col);
            this.col = col;
            this.Iplist = Iplist;
            ew = new EventWaitHandle(false, EventResetMode.ManualReset);
            w = false;
            ew.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="I"></param>
        public override void sender(bool self, int I)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="I"></param>
        /// <param name="b"></param>
        public override void add(int I, bool b)
        {
            CUP a = new CUP(b, I);
            lock (z)
            {
                if (r.ContainsKey(a.i))
                {
                    z.Add(a);
                }
                else
                {
                    r.Add(a.i, a.b);
                    if (w && col - 1 == r.Count)
                    {
                        ew.Set();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="I"></param>
        /// <returns></returns>
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
                if (!r.ContainsKey(z[i].i))
                {
                    r.Add(z[i].i, z[i].b);
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

        /// <summary>
        /// 
        /// </summary>
        ~SUPOneCenter()
        {
            ew.Close();
        }
    }
}
