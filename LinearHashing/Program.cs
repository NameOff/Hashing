using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearHashing
{
    class Program
    {
        public static void Main()
        {
            var line = new LinearHashing<int, int>();
            line.Add(8, 8);
            line.Add(13, 13);
            line.Add(10, 10);
            line.Add(15, 15);
            line.Add(19, 19);
            line.Add(22, 22);
            line.Add(18, 18);
            line.Remove(15);
            //Console.WriteLine(line[15]);
        }
    }
}
