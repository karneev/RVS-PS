namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Дескриптор данных для получения данных по коллекторной схеме обмена
        /// </summary>
        public class CollectorRecipientData
        {
            Recipient[] data;
            int I;

            /// <summary>
            /// Базовый конструктор класса
            /// </summary>
            /// <param name="data"> Данные </param>
            /// <param name="I"> Номер магины </param>
            public CollectorRecipientData(Recipient[] data, int I)
            {
                this.data = data;
                this.I = I;
            }

            /// <summary>
            /// Флаг получения
            /// </summary>
            /// <returns></returns>
            public bool isReady()
            {
                bool t = true;
                for (int i = 0; i < data.Length; i++)
                {
                    if (I != i) t = t && data[i].isReady();
                }
                return t;
            }

            /// <summary>
            /// Прием данные
            /// </summary>
            /// <param name="i"> Номер машины </param>
            /// <returns></returns>
            public object getData(int i)
            {
                if (i == I) return null;
                if (i < 0 || i >= data.Length) return null;
                return data[i].getData();
            }

            /// <summary>
            /// Блокировка потока 
            /// </summary>
            public void block()
            {
                for (int i = 0; i < data.Length; i++)
                    if (i != I) data[i].Block();
            }
        }
    }
}
