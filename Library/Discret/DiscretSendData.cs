using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Дескриптор данных для передачи данных по дифференцированной схеме обмена
        /// </summary>
        public class DiscretSendData
        {
            private object _data;
            private bool _ready;

            IPEndPoint Ip;
            EventWaitHandle ew;

            /// <summary>
            /// Данные
            /// </summary>
            public object data
            {
                get { return _data; }
                private set { _data = value; }
            }

            /// <summary>
            /// Флаг показывающий передались ли данные
            /// </summary>
            public bool ready
            {
                get { return _ready; }
                private set { _ready = value; }
            }

            /// <summary>
            /// Базовый конструктор
            /// </summary>
            /// <param name="Ip"> Адресс получателя </param>
            /// <param name="data"> Данные </param>
            public DiscretSendData(IPEndPoint Ip, object data)
            {
                this.Ip = Ip;
                this.data = data;
                ready = false;
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);
                ew.Reset();
                (new Thread(send)).Start();
            }

            /// <summary>
            /// Отправка данных
            /// </summary>
            void send()
            {
                byte[] buffer = new byte[255];
                TcpClient c = new TcpClient();
                //c.SendBufferSize = 2 * c.SendBufferSize;
                c.Connect(Ip);
                NetworkStream s = c.GetStream();
                if (s.ReadByte() == 1) return;
                BinaryFormatter bf = new BinaryFormatter();
                buffer[0] = 1;
                buffer[1] = 1;
                s.Write(buffer, 0, 255);
                bf.Serialize(s, data);
                ready = true;
                ew.Set();
                s.Close();
                ew.Close();
            }

            /// <summary>
            /// Блокировка потока
            /// </summary>
            public void block()
            {
                if (!ready) ew.WaitOne();
            }
        }
    }
}
