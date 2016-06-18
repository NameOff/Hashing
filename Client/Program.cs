using System;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.Connect("127.0.0.1", 9000);
            while (true)
            {
                var message = Console.ReadLine();
                if (message == "exit" || message == "")
                    return;
                Console.WriteLine(client.SendMessage(message));
            }
        }
    }
}
