using System;
using System.Linq;
using System.IO;

namespace SpliterSLAU
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader A = new StreamReader(args[0]);
            StreamReader B = new StreamReader(args[1]);
            int count = Convert.ToInt32(args[2]);
            int N = Convert.ToInt32(A.ReadLine());
            B.ReadLine();
            StreamWriter[] filesA = new StreamWriter[count];
            StreamWriter[] filesB = new StreamWriter[count];
            for (int i = 0; i < count; i++)
            {
                Directory.CreateDirectory(i.ToString());
                filesA[i] = new StreamWriter("./" + i + "/" + args[0].ToString());
                filesB[i] = new StreamWriter("./" + i + "/" + args[1].ToString());
                filesA[i].WriteLine(N);
                filesB[i].WriteLine(N);
                for (int j = 0; j < N / count; j++)
                {
                    string str = A.ReadLine();
                    filesA[i].WriteLine(str);
                    str = B.ReadLine();
                    filesB[i].WriteLine(str);
                }
                filesA[i].Close();
                filesB[i].Close();
            }
            A.Close();
            B.Close();
        }
    }
}