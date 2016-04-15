using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Вспомогательный класс. 
        /// Контейнер для желающих подключиться и передать информацию
        /// </summary>
        public class Connect
        {
            private int _index, _type;
            private object _data;
            private bool _ready = false;

            private Thread LoadThread;
            private EventWaitHandle ew;
            private NetworkStream socket;

            /// <summary>
            /// Номер машины
            /// </summary>
            public int Index
            {
                get { return _index; }
                private set { _index = value; }
            }

            /// <summary>
            /// Тип соединения
            /// </summary>
            public int Type
            {
                get { return _type; }
                private set { _type = value; }
            }

            /// <summary>
            /// Флаг отрадающий получены ли данные
            /// </summary>
            public bool Ready
            {
                get { return _ready; }
                private set { _ready = value; }
            }

            /// <summary>
            /// Загруженные данные
            /// </summary>
            public object Data
            {
                get
                {
                    if (Ready) return _data;
                    else return null;
                }
                private set { _data = value; }
            }

            /// <summary>
            /// Базовый консруктор класса
            /// </summary>
            /// <param name="_socket"></param>
            /// <param name="_type"></param>
            /// <param name="_i"></param>
            public Connect(NetworkStream _socket, int _type, int _i)
            {
                socket = _socket;
                Type = _type;
                Index = _i;
                Data = null;

                // Выполняет инициализацию нового экземпляра класса EventWaitHandle, 
                // определяя, получает ли сигнал, ожидающий дескриптор, 
                // и производится ли сброс автоматически или вручную.
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);

                // Задает несигнальное состояние события, вызывая блокирование потоков.
                ew.Reset();

                // запуск основного потока - метода run
                LoadThread = new Thread(Run);
                LoadThread.Start();
            }

            /// <summary>
            /// Функция - основной поток
            /// Получение данных и их десериализация 
            /// с помещение в переменную data для хранения
            /// </summary>
            private void Run()
            {
                // получение данных по сокету и десериализация
                BinaryFormatter b = new BinaryFormatter();
                Data = b.Deserialize(socket);
                Ready = true;
                // Задает сигнальное состояние события, 
                // позволяя одному или нескольким ожидающим потокам продолжить.
                ew.Set();

                // Освобождаем ресурсы
                socket.Close();
                ew.Close();
            }

            /// <summary>
            /// Блокировка потока
            /// </summary>
            public void Block()
            {
                // Блокирует текущий поток до получения сигнала объектом WaitHandle.
                if (!Ready) ew.WaitOne();
            }
        }
    }
}
