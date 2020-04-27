using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIChatClient
{
    class Client
    {
        private IPEndPoint IpPoint { get; set; }
        public bool Work { get; set; }
        private Socket Socket { get; set; }
        public string Name { get; set; }

        public Client(int port, string address)
        {
            IpPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }

        public void Start()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(IpPoint);
            Work = true;
            Task processingReceive = new Task(ProcessingReceive);
            processingReceive.Start();
        }

        private void ProcessingReceive()
        {
            while (Work)
            {
                try
                {
                    // Буффер для данных 
                    byte[] data;
                    string jsonString;
                    // Получаем размер сообщения
                    byte[] dataSize = new byte[4];
                    int sizeRequest;
                    Socket.Receive(dataSize, 4, SocketFlags.None);
                    sizeRequest = BitConverter.ToInt32(dataSize, 0);
                    // Получаем Json сообщение пользователя
                    data = new byte[sizeRequest];
                    Socket.Receive(data, data.Length, SocketFlags.None);
                    // Десериализация JSON при получении
                    jsonString = Encoding.UTF8.GetString(data);
                    Message message = JsonSerializer.Deserialize<Message>(jsonString);
                    //
                    Console.WriteLine(message.Name + ": "+ message.Text);
                    //
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                    Console.WriteLine($"Метод: {ex.TargetSite}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                    Work = false;
                }
            }
        }

        public bool SendMessage(string textMessage)
        {
            try
            {
                // Буффер для данных
                byte[] data;
                string jsonString;
                // Создание класса для сериализации
                Message message = new Message()
                {
                    Name = Name,
                    Text = textMessage
                };
                // Сериализация JSON для отправки
                jsonString = JsonSerializer.Serialize(message);
                data = Encoding.UTF8.GetBytes(jsonString);
                data = BitConverter.GetBytes(data.Length).Concat(data).ToArray();
                Socket.Send(data);
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

        public void Stop()
        {
            Socket.Close();
            Work = false;
        }
    }
}
