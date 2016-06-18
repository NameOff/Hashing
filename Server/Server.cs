using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server<TKey, TValue>
    {
        private readonly Socket socket;
        public IPEndPoint ServerInfo;
        private static Hashing.IDictionary<TKey, TValue> Dictionary;

        public Server(Hashing.IDictionary<TKey, TValue> dictionary, int port, int clientsCount)
        {
            if (clientsCount < 1)
                throw new ArgumentException();
            Dictionary = dictionary;
            ServerInfo = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ServerInfo);
            socket.Listen(clientsCount);
            MainLoop();
        }

        private void MainLoop()
        {
            while (true)
            {
                var sock = socket.Accept();
                Console.WriteLine("New client connected");
                new Thread(HandleClient).Start(sock);
            }
        }

        public static T ConvertTo<T>(string value)
        {
            return (T) Convert.ChangeType(value, typeof(T));
        }

        private static string HandleMessage(string message)
        {
            var words = message.Split();
            lock (Dictionary)
            {
                try
                {
                    switch (words[0].ToLower())
                    {
                        case "set":
                            Dictionary[ConvertTo<TKey>(words[1])] = ConvertTo<TValue>(words[2]);
                            return "OK";
                        case "get":
                            var value = Dictionary[ConvertTo<TKey>(words[1])];
                            return $"Value: {value}";
                        case "remove":
                            Dictionary.Remove(ConvertTo<TKey>(words[1]));
                            return "OK";
                        default:
                            return "Fail";
                    }
                }
                catch
                {
                    return "Fail";
                }
            }
        }

        private static void HandleClient(object socket)
        {
            var sock = (Socket)socket;
            while (true)
            {
                try
                {
                    var message = ReceiveMessage(sock);
                    if (message.Length == 0 || message == "exit")
                        throw new SocketException();

                    var answer = HandleMessage(message);
                    SendMessage(sock, answer);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Connection closed");
                    break;
                }
            }
        }

        private static string ReceiveMessage(Socket sock)
        {
            var data = new byte[1024];
            var bytes = sock.Receive(data, data.Length, 0);
            return Encoding.UTF8.GetString(data, 0, bytes);
        }

        private static void SendMessage(Socket sock, string message)
        {
            sock.Send(Encoding.UTF8.GetBytes(message));
        }
    }
}
