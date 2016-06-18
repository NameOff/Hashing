using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Client
    {
        private Socket socket;

        public void Connect(string ip, int port)
        {
            var data = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(data);
        }

        public string SendMessage(string message)
        {
            try
            {
                socket.Send(Encoding.UTF8.GetBytes(message));
                var data = new byte[1024];
                var bytes = socket.Receive(data, data.Length, 0);
                return Encoding.UTF8.GetString(data, 0, bytes);
            }
            catch (SocketException)
            {
                return null;
            }
        }

        public void CloseConnection()
        {
            SendMessage("exit");
        }

    }
}
