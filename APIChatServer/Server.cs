using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace APIChatServer
{
    class Server
    {
        public List<User> Clients = new List<User>();
        private IPEndPoint IPEndPoint { get; set; }
        private bool Work { get; set; }
        private int ClientIp { get; set; }

        public Server(int port, string address)
        {
            IPEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }

        public bool Start()
        {
            try
            {
                Work = true;
                Task listener = new Task(Listener);
                listener.Start();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                return false;
            }
        }

        private void Listener()
        {
            try
            {
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(IPEndPoint);
                listener.Listen(40);
                Console.WriteLine("Сервер чата запущен.");
                while (Work)
                {
                    User user = new User
                    {
                        Socket = listener.Accept(),
                        Server = this,
                        Id = GetClientIp()
                    };
                    Clients.Add(user);
                    Task userTask = new Task(user.Processing);
                    userTask.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
            }
            Console.WriteLine("Сервер чата выключен.");
        }

        public bool Stop()
        {
            try
            {
                Work = false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                return false;
            }
        }

        public int GetClientIp()
        {
            ClientIp++;
            return ClientIp;
        }

        /// <summary>
        /// Отправка сообщения всем пользователям подключенным к чату
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendEveryone(byte[] data)
        {
            try
            {
                foreach (User client in Clients)
                {
                    client.Socket.Send(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                return false;
            }
        }
    }
}
