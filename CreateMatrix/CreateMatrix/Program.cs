using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CreateMatrix
{
    class Program
    {
        static int N = 1000;
        static double er = 0.0000001;
        static int max = 10000000;
        static void Main(string[] args)
        {
            Random r = new Random((int)System.DateTime.Now.Ticks);
            double dd = 0;
            StreamWriter R = new StreamWriter("Work.txt");
            R.WriteLine(er);
            R.WriteLine(N);
            double[] A=new double[N];
            for (int i = 0; i < N; i++)
            {
                Console.WriteLine("I crete col {0:D}", i);
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
                Console.WriteLine("creted");
            }
            for (int i = 0; i < N; i++)
            {
                R.Write(r.Next((int)(max * 0.7), max));
                if (i != N - 1) R.Write(" ");
            }
            R.Close();
            /*  for (int i = 0; i < N; i++)
              {
                  for (int j = 0; j < N; j++)
                      Console.Write("{0:F}, ",A[i][j]);
                  Console.WriteLine();
              }*/
        }
    }
}
