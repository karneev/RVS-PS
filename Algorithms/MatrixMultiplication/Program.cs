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
        bool end = true;
        StreamReader R = new StreamReader("A.txt");
        int N = Convert.ToInt32(R.ReadLine());
        int ProcPartSize = N / getCount();
        double[][] A = new double[ProcPartSize][];
        for (int i = 0; i < ProcPartSize; i++)
        {
            A[i] = new double[N];
        }
        double[][] B = new double[ProcPartSize][];
        for (int i = 0; i < ProcPartSize; i++)
        {
            B[i] = new double[N];
        }
        double[][] BuffC = new double[ProcPartSize][];
        for (int i = 0; i < ProcPartSize; i++)
        {
            BuffC[i] = new double[N];
        }
        double[][] buffer;
        DateTime check;
        int pred = (getIndex() == 0) ? getCount() - 1 : getIndex() - 1;
        int next = (getIndex() == getCount() - 1) ? 0 : getIndex() + 1;
        DateTime time = System.DateTime.Now;
        int ic = 0;
        for (; ic < (N / getCount()) * getIndex(); R.ReadLine(), ic++); //Пропуск ненужных машине строк
        for (int i = 0; i < ProcPartSize; i++)
        {
            A[i] = R.ReadLine().Split(' ').Select(Double.Parse).ToArray();
        }
        R.Close();
        R = new StreamReader("B.txt");
        N = Convert.ToInt32(R.ReadLine());
        ic = 0;
        for (; ic < (N / getCount()) * getIndex(); R.ReadLine(), ic++) ;
        for (int i = 0; i < ProcPartSize; i++)
        {
            B[i] = R.ReadLine().Split(' ').Select(Double.Parse).ToArray();
        }
        R.Close();
        DateTime time1 = System.DateTime.Now;
        check = System.DateTime.Now;
        do
        {
            buffer = B;
            double sum = 0;
            int ind;
            for (int m = 0; m < getCount(); m++)
            {
                for (int i = 0; i < ProcPartSize; i++)
                {
                    for (int j = 0; j < ProcPartSize; j++)
                    {
                        for (int k = 0; k < N; k++)
                        {
                            sum += A[i][k] * buffer[j][k];
                        }
                        if (getIndex() - m >= 0)
                            ind = getIndex() - m;
                        else ind = (getCount() - m + getIndex());
                        BuffC[i][j+ind*ProcPartSize] = sum;
                        sum = 0;
                    }
                }
                DiscretRecipientData R1 = GetData(pred);
                DiscretSendData R2 = Send(next, buffer);
                R1.block();
                buffer = (double[][])R1.getData();
            }
            end = true;
        } while (!SGCJ(end)); //Точка синхронизации всех машин
        if (getIndex() != 0)
        {
            CollectSendData cl = CollectorSend(0, BuffC);
            cl.block();
            Console.WriteLine("CollectSend " + getIndex() + "part");
        }
        else
        {
            double[][] C = new double[N][];
            for (int i = 0; i < N; i++)
                C[i] = new double[N];
            for (int i = 0; i < ProcPartSize; i++)
                C[i] = BuffC[i];
            CollectorRecipientData r = getAllData();
            r.block();
            for (int i = 1; i < getCount(); i++)
            {
                double[][] data = (double[][])r.getData(i);
                for (int k = 0; k < ProcPartSize; k++)
                    C[i*ProcPartSize+k] = data[k];
            }
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                    Console.Write(C[i][j] + " ");
                Console.WriteLine();
            } 
        }
        DateTime check1 = System.DateTime.Now;
        Console.WriteLine("Reading time {0}", (time1 - time).TotalMilliseconds);
        Console.WriteLine("Computing time {0}", (check1 - check).TotalMilliseconds);
        Console.ReadKey();
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
