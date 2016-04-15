using System.Threading;
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Выенесен из Work!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
namespace Library
{
    /// <summary>
    /// Класс шаблон
    /// </summary>
    abstract class SUP
    {
        /// <summary>
        /// 
        /// </summary>
        protected class CUP
        {
            private bool _b;
            private int _i;

            /// <summary>
            /// 
            /// </summary>
            public bool b
            {
                get { return _b; }
                set { _b = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public int i
            {
                get { return _i; }
                set { _i = value; }
            }

            /// <summary>
            /// Базовый конструктор класса
            /// </summary>
            /// <param name="_b"></param>
            /// <param name="_i"></param>
            public CUP(bool _b, int _i)
            {
                b = _b;
                i = _i;
            }
        }

        protected int col;
        protected bool w;
        protected EventWaitHandle ew;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="I"></param>
        /// <param name="b"></param>
        public abstract void add(int I, bool b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="I"></param>
        /// <returns></returns>
        public abstract bool collect(bool self, int I);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="I"></param>
        public abstract void sender(bool self, int I);
    }
}
