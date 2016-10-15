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
    public mainFrame(int port = 8002) : base(port) { }
    public override void slaveFun()
    {
        bool end = true;
        StreamReader R = new StreamReader("A.txt");
        int N = Convert.ToInt32(R.ReadLine());
        int ProcPartSize = N / getCount();
        float[][] A = new float[ProcPartSize][];
        for (int i = 0; i < ProcPartSize; i++)
        {
            A[i] = new float[N];
        }
        float[][] B = new float[ProcPartSize][];
        for (int i = 0; i < ProcPartSize; i++)
        {
            B[i] = new float[N];
        }
        float[][] BuffC = new float[ProcPartSize][];
        for (int i = 0; i < ProcPartSize; i++)
        {
            BuffC[i] = new float[N];
        }
        float[][] buffer;
        DateTime check;
        int pred = (getIndex() == 0) ? getCount() - 1 : getIndex() - 1;
        int next = (getIndex() == getCount() - 1) ? 0 : getIndex() + 1;
        DateTime time = System.DateTime.Now;
        for (int i = 0; i < ProcPartSize; i++)
        {
            A[i] = R.ReadLine().Split(' ').Select(float.Parse).ToArray();
        }
        R.Close();
        R = new StreamReader("B.txt");
		R.ReadLine();
        for (int i = 0; i < ProcPartSize; i++)
        {
            B[i] = R.ReadLine().Split(' ').Select(float.Parse).ToArray();
        }
        R.Close();
        DateTime time1 = System.DateTime.Now;
        check = System.DateTime.Now;
        do
        {
            buffer = B;
            float sum = 0;
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
                buffer = (float[][])R1.getData();
            }
            end = true;
        } while (!SGCJ(end)); //Точка синхронизации всех машин
        if (getIndex() != 0)
        {
            CollectSendData cl = CollectorSend(0, BuffC);
            cl.block();
        }
        else
        {
            CollectorRecipientData r = getAllData();
            r.block();
			StreamWriter f = new StreamWriter("result.txt");
			for (int i = 0; i < ProcPartSize; i++)
			{
				for (int j = 0; j < N; j++)
					f.Write(BuffC[i][j]+" ");
				f.WriteLine();
			}
			for (int i = 1; i < getCount(); i++)
			{
				float[][] data = (float[][])r.getData(i);
				for (int j = 0; j < ProcPartSize; j++)
				{
					for (int k = 0; k < N; k++)
						f.Write(data[j][k]+" ");
					f.WriteLine();
				}
			}
        DateTime check1 = System.DateTime.Now;
        f.WriteLine("Reading time {0} ms", (time1 - time).TotalMilliseconds);
        f.WriteLine("Computing time {0} ms", (check1 - check).TotalMilliseconds);
        f.Close();
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
