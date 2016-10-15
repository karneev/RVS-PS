using System;
using System.IO;

namespace MatrixGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = Convert.ToInt32(Console.ReadLine());
            Random r = new Random();
            StreamWriter a = new StreamWriter("A.txt");
            StreamWriter b = new StreamWriter("B.txt");
            a.WriteLine(N);
            b.WriteLine(N);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    a.Write(r.Next(100));
                    if (j != N - 1) a.Write(" ");
                    b.Write(r.Next(100));
                    if (j != N - 1) b.Write(" ");
                }
                a.WriteLine();
                b.WriteLine();
            }
            a.Close();
            b.Close();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
