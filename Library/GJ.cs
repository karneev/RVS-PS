using System.Collections;

namespace Library
{
    partial class Work
    {
        /// <summary>
        ///  Класс для хранения функций, которые возможно нуждаются в прерываниях 
        /// </summary>
        class GJ
        {
            Hashtable DF;

            /// <summary>
            /// Базовый конструктор класса
            /// </summary>
            public GJ()
            {
                DF = new Hashtable();
            }

            /// <summary>
            /// Установим функцию для прерывания
            /// </summary>
            /// <param name="i"></param>
            /// <param name="d"></param>
            public void Set(int i, delfunc d)
            {
                if (DF.ContainsKey(i)) DF.Remove(i);
                DF.Add(i, d);
            }

            /// <summary>
            /// Получить эту функцию
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public delfunc Get(int i)
            {
                if (!DF.ContainsKey(i)) return null;
                else return (delfunc)DF[i];
            }
        }
    }
}
