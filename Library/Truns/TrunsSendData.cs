using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Дескриптор данных для передачи данных по трансляционной схеме обмена
        /// </summary>
        public class TrunsSendData
        {
            List<IPEndPoint> Ip;
            int I;
            object data;
            int count;
            EventWaitHandle ew;

            /// <summary>
            /// Базовый конструктор класса
            /// </summary>
            /// <param name="Ip"> Список адрессов для приема</param>
            /// <param name="I"> Номер текущей машины </param>
            /// <param name="data"> Данные для отправки </param>
            public TrunsSendData(List<IPEndPoint> Ip, int I, object data)
            {
                this.Ip = Ip;
                this.I = I;
                this.data = data;
                count = Ip.Count - 1;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                for (int i = 0; i < Ip.Count; i++)
                {
                    if (i != I) (new Thread(send)).Start(i);
                }
            }

            /// <summary>
            /// Отправка данных
            /// </summary>
            /// <param name="ii"> Номер машины которой отправляем </param>
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
                s.Write(buffer, 0, 255);
                bf.Serialize(s, data);
                lock (data)
                {
                    count--;
                    if (count == 0)
                    {
                        ew.Set();
                        ew.Close();
                    }
                }
                s.Close();
                c.Close();
            }

            /// <summary>
            /// Флаг свидетельствующий об конце отправки данных
            /// </summary>
            /// <returns></returns>
            public bool isReady()
            {
                return count == 0;
            }

            /// <summary>
            /// Блокировка потока
            /// </summary>
            public void block()
            {
                if (!(count == 0)) ew.WaitOne();
            }
        }
    }
}
