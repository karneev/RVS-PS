namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Дескриптор данных для получения данных по дифференцированной схеме обмена
        /// </summary>
        public class DiscretRecipientData
        {
            Recipient r;

            /// <summary>
            /// Базовый конструктор
            /// </summary>
            /// <param name="r"> Объект контейнера получения данных </param>
            public DiscretRecipientData(Recipient r)
            {
                this.r = r;
            }

            /// <summary>
            /// Получения состояния получателя
            /// </summary>
            /// <returns></returns>
            public bool isReady()
            {
                return r.isReady();
            }

            /// <summary>
            /// Получение данных
            /// </summary>
            /// <returns></returns>
            public object getData()
            {
                return r.getData();
            }

            /// <summary>
            /// Номер машины
            /// </summary>
            /// <returns></returns>
            public int getI()
            {
                return r.Index;
            }

            /// <summary>
            /// Тип соединения
            /// </summary>
            /// <returns></returns>
            public int getType()
            {
                return r.Type;
            }

            /// <summary>
            /// Блокировка потока
            /// </summary>
            public void block()
            {
                r.Block();
            }
        }
    }
}
