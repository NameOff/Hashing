using Hashing;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server<int, int>(new EHashMap<int,int>(), 9000, 10);
        }
    }
}
