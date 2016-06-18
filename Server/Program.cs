using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Hashing;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server<int, int>(new LHashMap<int,int>(), 9000, 10);
            //Server<int, int>.ConvertTo<bool>("true");
        }
    }
}
