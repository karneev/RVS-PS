using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    partial class Work
    {
        /// <summary>
        /// Контейнер для соединения соответствующих сокетов и потоков обработки принятия данных
        /// </summary>
        class Spindle
        {
            // список контейнеров с данными
            List<Connect> In;
            // список поток принятия данных
            List<Recipient> Out;

            /// <summary>
            /// Базовый конструктор
            /// </summary>
            public Spindle()
            {
                In = new List<Connect>();
                Out = new List<Recipient>();
            }

            /// <summary>
            /// Получение признака нахождения объекта в списке
            /// </summary>
            /// <param name="connectObject"></param>
            /// <returns> 1 если нет, 0 если есть </returns>
            public int Get(Connect connectObject)
            {
                // если список пуст добавим connectObject в список
                if (Out.Count == 0)
                {
                    In.Add(connectObject);
                    return 1;
                }

                // ищем в списке такой же объект как и connectObject
                int dd = -1;
                for (int i = 0; i < Out.Count; i++)
                {
                    if (connectObject.Index == Out[i].Index && connectObject.Type == Out[i].Type)
                    {
                        dd = i;
                        break;
                    }
                }

                // если не нашли, то добавим connectObject в список
                if (dd == -1)
                {
                    In.Add(connectObject);
                    return 1;
                }

                // иначе берем этот объект, удаляем из списка и устанавливаем объект соединения на нем
                Recipient r = Out[dd];
                Out.RemoveAt(dd);
                r.setConnectObject(connectObject);
                return 0;
            }

            /// <summary>
            /// Получение признака нахождения объекта в списке
            /// </summary>
            /// <param name="recipientObject"></param>
            /// <returns> 1 если нет, 0 если есть </returns>
            public int Get(Recipient recipientObject)
            {
                // если список пуст то добавим recipientObject в список
                if (In.Count == 0)
                {
                    Out.Add(recipientObject);
                    return 1;
                }

                // ищем в списке такой же объект как и recipientObject
                int dd = -1;
                for (int i = 0; i < In.Count; i++)
                {
                    if (recipientObject.Type == In[i].Type && recipientObject.Index == In[i].Index)
                    {
                        dd = i;
                        break;
                    }
                }

                // если не нашли, то добавим recipientObject в список
                if (dd == -1)
                {
                    Out.Add(recipientObject);
                    return 1;
                }

                // иначе удалим из списка этот объект и установим соединение 
                Connect c = In[dd];
                In.RemoveAt(dd);
                recipientObject.setConnectObject(c);
                return 0;
            }
        }
    }
}
