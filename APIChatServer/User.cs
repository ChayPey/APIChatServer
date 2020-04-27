using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace APIChatServer
{
    class User
    {
        public Socket Socket { get; set; }
        public Server Server { get; set; }
        public int Id { get; set; }
        public bool Work { get; set; }

        // Прием сообщения от пользователя
        public void Processing()
        {
            Work = true;
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
                    // Изменение JSON перед отправкой
                    message.Id = Id;
                    // Сериализация JSON для отправки
                    jsonString = JsonSerializer.Serialize(message);
                    data = Encoding.UTF8.GetBytes(jsonString);
                    data = BitConverter.GetBytes(data.Length).Concat(data).ToArray();
                    // Отправка сообщения всем подключенным пользователям
                    Server.SendEveryone(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                    Console.WriteLine($"Метод: {ex.TargetSite}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                    ExitUser();
                    Work = false;
                }
            }
        }

        // Удаления пользователя из списка пользователей сервера
        private void ExitUser()
        {
            Server.Clients.Remove(this);
        }
    }
}
