using System;
using System.Linq;
using Library;
using System.IO;

/// <summary>
/// Класс наследник базового класса библиотеки - реализация алгоритма
/// </summary>

class mainFrame : Work
{
    /// <summary>
    /// Реалиация алгоритма
    /// </summary>
    public override void slaveFun()
    {
        double checker = 0;
        DateTime check;
        #region Чтение из файла
        StreamReader R = new StreamReader("Work.txt");
        double er = (double)Convert.ToDouble(R.ReadLine());
        int N = Convert.ToInt32(R.ReadLine());
        double[][] A = new double[N / getCount()][];
        int ic = 0;
        for (; ic < (N / getCount()) * getIndex(); R.ReadLine(), ic++) ;
        for (int i = 0; ic < (N / getCount()) * (getIndex() + 1) && ic < N; i++, ic++)
        {
            A[i] = R.ReadLine().Split(new char[] { ' ' }).Select(Double.Parse).ToArray();
        }
        for (; ic < N; R.ReadLine(), ic++) ;
        double[] F = R.ReadLine().Split(new char[] { ' ' }).Skip(getIndex() * N / getCount()).Take(N / getCount()).Select(Double.Parse).ToArray();
        double[] X = new double[F.Length];
        double[] timeX = new double[F.Length];
        double[] buffer;
        bool end = true;
        int JJJ = N / getCount();
        int JJJ1 = getIndex() * JJJ;
        #endregion
        Console.WriteLine("Start");
        DateTime time = System.DateTime.Now;
        int it = 0;
        int pred = (getIndex() == 0) ? getCount() - 1 : getIndex() - 1, next = (getIndex() == getCount() - 1) ? 0 : getIndex() + 1;
        int iterr = 0;
        do
        {
            //check = DateTime.Now;
            for (int i = 0; i < F.Length; i++)
            {
                timeX[i] = F[i];
            }
            buffer = X;
            int w = getIndex();
            //checker += (System.DateTime.Now - check).TotalMilliseconds;
            for (int m = 0; m < getCount(); m++)
            {
                int JJJ2 = w * JJJ;
                Work.DiscretRecipientData R1 = GetData(pred);
                Work.DiscretSendData R2 = Send(next, buffer);
                check = System.DateTime.Now;
                for (int i = 0; i < timeX.Length; i++)
                {
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        iterr++;
                        if ((i + JJJ1) != (j + JJJ2)) timeX[i] -= A[i][(JJJ2) + j] * buffer[j];
                    }
                }
                w = (w + 1 == getCount()) ? 0 : w + 1;
                checker += (System.DateTime.Now - check).TotalMilliseconds;
                R1.block();

                buffer = (double[])R1.getData();// Thread.Sleep(5000);

            }
            end = true;
            // check = DateTime.Now;
            for (int i = 0; i < JJJ; i++)
            {
                timeX[i] /= A[i][i + JJJ * getIndex()];
                end = end && (er > Math.Abs(X[i] - timeX[i]));
                X[i] = timeX[i];
            }
            // checker += (System.DateTime.Now - check).TotalMilliseconds;
            it++;
        } while (!SGCJ(end));
        DateTime time1 = System.DateTime.Now;
        for (int i = 0; i < JJJ; i++)
        {
            Console.WriteLine("X{0:D}:{1:E}", getIndex() * JJJ + i, X[i]);
        }
        Console.Write("Time work: ");
        Console.WriteLine((time1 - time).TotalMilliseconds);
        Console.WriteLine(checker);
        Console.WriteLine(iterr);
        Console.ReadLine();
    }
}

/// <summary>
/// Класс программы - запуск алгоритма
/// </summary>
class Program
{
    public static void Main(string[] args)
    {
        mainFrame m = new mainFrame();
        m.setSUP(0);
        m.start();
    }
}
