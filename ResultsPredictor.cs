using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsPredictor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input number of PC (from 1 till 8): ");
            int N = int.Parse(Console.ReadLine());
            Console.WriteLine("Input size (from 10k till 30k)");
            int M = int.Parse(Console.ReadLine());
            int k = M * M / 10 / 10;
            k = 1;
            double[] A = new double[N];
            double s = 0;
            double s1 = 0;
            double max = 0;
            for (int i=0; i< N; i++)
            {
                Console.WriteLine("Input test result from computer" + i.ToString());
                A[i] = double.Parse(Console.ReadLine());
                s+= A[i];
                s1 += 1 / A[i];
                if (A[i] > max)
                    max = A[i];
            }
            double r1 = 1 / s1; // идеально сбалансированный результат
            double r2 = 1 / (N * 1/max); // идеально несбалансированный результат

            Console.WriteLine("Ideal balansed result: " + (r1* k).ToString());
            Console.WriteLine("Ideal unbalansed result: " + (r2*k).ToString());
            Console.WriteLine("Time lost: " + (r2 - r1).ToString());
            Console.WriteLine("Real balansed result: " + ((r1/0.8)* k).ToString());
            Console.WriteLine("Real unbalansed result: " + ((r2/0.8)* k).ToString());
            Console.ReadLine();
           }
    }
}
