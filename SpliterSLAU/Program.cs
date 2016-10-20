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
            int cntI = N/count;   //Сколько дать каждой машине (по умолчанию)
            for (int i = 0; i < count; i++)
            {
                string mdir = args[0].Split('.')[0];
                Directory.CreateDirectory(mdir+"/"+i.ToString());
                files[i] = new StreamWriter("./"+ mdir + "/" + i + "/"+args[0]);
                files[i].WriteLine(err);
                if (args.Length == 2)
                    files[i].WriteLine(N/count);
                else
                {   //Части не равны
                    cntI = Convert.ToInt32(args[i + 2]);
                    files[i].WriteLine(cntI);
                }
                for (int j = 0; j < cntI; j++)
                {
                    files[i].WriteLine(R.ReadLine());
                }
            }
            string str = R.ReadLine();
            R.Close();
            for (int i = 0; i < count; i++)
            {
                string[] s = str.Split(new char[] { ' ' }).Skip(i * cntI).Take(cntI).ToArray();
                files[i].WriteLine(String.Join(" ", s));
                files[i].Close();
            }
        }
    }
}
