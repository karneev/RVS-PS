using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CreateMatrix
{
    class Program
    {
        static double er = 0.0000001;
        static int max = 10000000;
        static void Main(string[] args)
        {
            int N;
            N = Int32.Parse(Console.ReadLine());
            Random r = new Random((int)System.DateTime.Now.Ticks);
            StreamWriter R = new StreamWriter("Work.txt");
            R.WriteLine(er);
            R.WriteLine(N);
            double[] A=new double[N];
            for (int i = 0; i < N; i++)
            {
                if ((N>100)&&(i % (N/100) == 0))
                {
                    Console.Clear();
                    Console.WriteLine((i / (N / 100)+1).ToString() + " % ready");
                }
                double Aii = r.Next((int)(max * 0.7), max);
                int summ=0;
                for (int j = 0; j < N; j++)
                    if (i != j)
                    {
                        A[j] = r.Next((int)(Aii / N));
                        summ += (int)Math.Abs(A[j]);
                    }
                    else A[j] = Aii;
                if (summ > Aii) i-=1;
                else
                {
                    for (int j = 0; j < N; j++)
                    {
                        R.Write(A[j]);
                        if (j != N - 1) R.Write(" ");
                    }
                    R.WriteLine();
                }
            }
            for (int i = 0; i < N; i++)
            {
                R.Write(r.Next((int)(max * 0.7), max));
                if (i != N - 1) R.Write(" ");
            }
            R.Close();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
