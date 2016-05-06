using System;
using System.Linq;
using System.IO;

namespace SpliterSLAU
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader R = new StreamReader(args[0]);
            int count = Convert.ToInt32(args[1]);
            double err = (double)Convert.ToDouble(R.ReadLine());
            int N = Convert.ToInt32(R.ReadLine());
            StreamWriter[] files = new StreamWriter[count];
            for (int i = 0; i < count; i++)
            {
                string mdir = args[0].Split('.')[0];
                Directory.CreateDirectory(mdir+"/"+i.ToString());
                files[i] = new StreamWriter("./"+ mdir + "/" + i + "/"+args[0]);
                files[i].WriteLine(err);
                files[i].WriteLine(N);
                for (int j = 0; j < N / count; j++)
                {
                    files[i].WriteLine(R.ReadLine());
                }
            }
            string str = R.ReadLine();
            R.Close();
            for (int i = 0; i < count; i++)
            {
                string[] s = str.Split(new char[] { ' ' }).Skip(i * N / count).Take(N / count).ToArray();
                files[i].WriteLine(String.Join(" ", s));
                files[i].Close();
            }
        }
    }
}
