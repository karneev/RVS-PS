namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Дескриптор данных для приема данных по трансляционной схеме обмена
        /// </summary>
        public class TrunsRecipientData
        {
            Recipient r;

            /// <summary>
            /// Базовый конструктор класса
            /// </summary>
            /// <param name="r"> Объект класса приемника </param>
            public TrunsRecipientData(Recipient r)
            {
                this.r = r;
            }

            /// <summary>
            /// Флаг свидетельствующий об окончании передачи
            /// </summary>
            /// <returns></returns>
            public bool isReady()
            {
                return r.isReady();
            }

            /// <summary>
            /// Данные
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
            /// Тип подключения
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
