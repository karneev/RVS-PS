using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
namespace Library
{
    /// <summary>
    /// Способ реализации модуля обобщеного условного перехода "ВСЕ ОДНОМУ" для для других машин
    /// </summary>
    class SUPOneLeft : SUP
    {
        IPEndPoint res;
        bool rezalt/*, ready*/;
        object Block;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        public SUPOneLeft(IPEndPoint res)
        {
            this.res = res;
            this.col = 0;
            w = false;
            Block = new Object();
            ew = new EventWaitHandle(false, EventResetMode.ManualReset);
            ew.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="I"></param>
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
            c.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="I"></param>
        /// <param name="b"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="I"></param>
        /// <returns></returns>
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
}
