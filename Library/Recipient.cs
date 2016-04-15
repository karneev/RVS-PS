using System.Threading;

namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Получатель - контейнер для потоков обрабатывающих принятие данных
        /// </summary>
        public class Recipient
        {
            private EventWaitHandle ew;
            private Connect _connectContainer = null;
            private int _index, _type;

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
            /// Контейнер сокета подключившейся машины
            /// </summary>
            public Connect connectContainer
            {
                get { return _connectContainer; }
                set { _connectContainer = value; }
            }

            /// <summary>
            /// Базовый конструктор класса
            /// </summary>
            /// <param name="_index"></param>
            /// <param name="_type"></param>
            public Recipient(int _index, int _type)
            {
                Index = _index;
                Type = _type;

                // Выполняет инициализацию нового экземпляра класса EventWaitHandle, 
                // определяя, получает ли сигнал, ожидающий дескриптор, 
                // и производится ли сброс автоматически или вручную.
                ew = new EventWaitHandle(false, EventResetMode.ManualReset);

                // Задает несигнальное состояние события, вызывая блокирование потоков.
                ew.Reset();
            }

            /// <summary>
            /// Установка базового объекта подключения
            /// </summary>
            /// <param name="obj"></param>
            public void setConnectObject(Connect obj)
            {
                connectContainer = obj;
                ew.Set();
                ew.Close();
            }

            /// <summary>
            /// Получение данных
            /// </summary>
            /// <returns></returns>
            public object getData()
            {
                if (connectContainer == null) return null;
                else return connectContainer.Data;
            }

            /// <summary>
            /// Проверка готовности
            /// </summary>
            /// <returns></returns>
            public bool isReady()
            {
                if (connectContainer != null) return connectContainer.Ready;
                else return false;
            }

            /// <summary>
            /// Блокировка потока
            /// </summary>
            public void Block()
            {
                if (connectContainer == null) ew.WaitOne();
                connectContainer.Block();
            }
        }
    }
}
