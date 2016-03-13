using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace testMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader R = new StreamReader("Work.txt");
            DateTime startProg = System.DateTime.Now;
            double er = Convert.ToDouble(R.ReadLine());
            int N = Convert.ToInt32(R.ReadLine());
            double[][] A = new double[N][];
            for (int i = 0; i < N; i++)
            {
                if (i % (N / 100) == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Reading " + (i / (N / 100)).ToString() + " %");
                }
                A[i] = R.ReadLine().Split(new char[] { ' ' }).Select(Convert.ToDouble).ToArray();
            }
            double[] F = R.ReadLine().Split(new char[] { ' ' }).Select(Convert.ToDouble).ToArray();
            double[] X = new double[N];
            double[] timeX = new double[N];
            bool end = true;
            int itera = 0;
            DateTime startComp = System.DateTime.Now;
            int iterr = 0;
            Console.WriteLine("Start");
            do
            {
                itera++;
                Console.Clear();
                Console.WriteLine("Iteration: " + itera.ToString());
                for (int i = 0; i < N; i++)
                {
                    timeX[i] = F[i];
                    for (int g = 0; g < N; g++)
                    {
                        iterr++;
                        if (i != g)
                            timeX[i] -= A[i][g] * X[g];
                    }
                    timeX[i] /= A[i][i];
                }
                end = true;
                for (int i = 0; i < N; i++)
                {
                    end = end && (er > Math.Abs(X[i] - timeX[i]));
                    X[i] = timeX[i];
                }
            } while (!end);
            Console.WriteLine("Done!");
            DateTime finish = System.DateTime.Now;
            Console.WriteLine("Total time: " + (finish - startProg).TotalSeconds);
            Console.WriteLine("Computing time: " + (finish - startComp).TotalSeconds);
            /* for (int i =0; i < N; i++)
             {
                 Console.WriteLine("X{0:D}:{1:E}",i,X[i]);
             }
             Console.WriteLine("Time work: {0:F}", (time1-time).TotalMilliseconds);
             Console.WriteLine("Iteration: {0:G}", itera);
             //Console.WriteLine(iterr);*/
            Console.ReadLine();
        }
    }
}
