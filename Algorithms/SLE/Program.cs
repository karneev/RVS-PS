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
    public mainFrame(int port = 8002) : base(port){}
    public override void slaveFun()
    {
        DateTime time1 = System.DateTime.Now;
        #region Чтение из файла
        Console.WriteLine("Reading");
        StreamReader R = new StreamReader("Work.txt");
        double er = (double)Convert.ToDouble(R.ReadLine());
        int N = Convert.ToInt32(R.ReadLine());
        double[][] A = new double[N][];
        //for (; ic < (N) * getIndex(); R.ReadLine(), ic++) ;
        for (int i = 0; i < N; i++)
        {
            A[i] = R.ReadLine().Split(new char[] { ' ' }).Select(double.Parse).ToArray();
        }
        //for (; ic < N; R.ReadLine(), ic++) ;
        double[] F = R.ReadLine().Split(new char[] { ' ' }).Select(double.Parse).ToArray();
        double[] X = new double[F.Length];
        double[] timeX = new double[F.Length];
        double[] buffer;
        bool end = true;
        int JJJ = N;
        int JJJ1 = getIndex() * JJJ;
        #endregion
        Console.WriteLine("Start");
        DateTime time2 = System.DateTime.Now;
        int it = 0;
        int pred = (getIndex() == 0) ? getCount() - 1 : getIndex() - 1, next = (getIndex() == getCount() - 1) ? 0 : getIndex() + 1;
        int iterr = 0;
        do
        {
            Console.Clear();
            Console.WriteLine("Iteration: " + it.ToString());
            for (int i = 0; i < F.Length; i++)
            {
                timeX[i] = F[i];
            }
            buffer = X;
            int w = getIndex();
            for (int m = 0; m < getCount(); m++)
            {
                int JJJ2 = w * JJJ;
                Work.DiscretRecipientData R1 = GetData(pred);
                Work.DiscretSendData R2 = Send(next, buffer);
                for (int i = 0; i < timeX.Length; i++)
                {
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        iterr++;
                        if ((i + JJJ1) != (j + JJJ2)) timeX[i] -= A[i][(JJJ2) + j] * buffer[j];
                    }
                }
                w = (w + 1 == getCount()) ? 0 : w + 1;
                R1.block();

                buffer = (double[])R1.getData();

            }
            end = true;
            for (int i = 0; i < JJJ; i++)
            {
                timeX[i] /= A[i][i + JJJ * getIndex()];
                end = end && (er > Math.Abs(X[i] - timeX[i]));
                X[i] = timeX[i];
            }
            it++;
        } while (!SGCJ(end));
        if (getIndex() != 0)
        {
            CollectSendData cl = CollectorSend(0, X);
            cl.block();
            Console.WriteLine("CollectSend " + getIndex() + " part");
        }
        else
        {
            StreamWriter sw = new StreamWriter("result.txt");
            CollectorRecipientData r = getAllData();
            r.block();
            DateTime time3 = System.DateTime.Now;
            for (int i = 0; i < JJJ; i++)
            {
                sw.WriteLine("X{0:D}:{1:E}", i, X[i]);
            }
            for (int i = 1; i < getCount(); i++)
            {
                double[] data = (double[])r.getData(i);
                for (int j = 0; j < JJJ; j++)
                {
                    sw.WriteLine("X{0:D}:{1:E}", i * JJJ + j, data[j]);
                }
            }
            sw.Write("Total (computing and reading) - seconds: ");
            sw.WriteLine((time3 - time1).TotalSeconds);
            sw.Write("Only computing - seconds: ");
            sw.WriteLine((time3 - time2).TotalSeconds);
            sw.Write("Write In File - seconds: ");
            sw.WriteLine((DateTime.Now - time1).TotalSeconds);
            sw.Close();
        }
    }
}

/// <summary>
/// Класс программы - запуск алгоритма
/// </summary>
class Program
{
    public static void Main(string[] args)
    {
        mainFrame m;
        if (args.Length == 0)
            m = new mainFrame();
        else
            m = new mainFrame(Convert.ToInt32(args[1]));
        m.setSUP(0);
        m.start();
    }
}
