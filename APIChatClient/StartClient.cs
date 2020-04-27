using System;

namespace APIChatClient
{
    class StartClient
    {
        static void Main(string[] args)
        {
            Client client = new Client(19970, "127.0.0.1");
            Console.WriteLine("Соединение установленно!");
            Console.WriteLine("Ваше имя: ");
            client.Name = Console.ReadLine();
            client.Start();
            while (true)
            {
                client.SendMessage(Console.ReadLine());
            }
            Console.ReadLine();
        }
    }
}
