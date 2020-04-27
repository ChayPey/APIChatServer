using System;

namespace APIChatServer
{
    class StartServer
    {
        static void Main(string[] args)
        {
            Server server = new Server(19970, "127.0.0.1");
            server.Start();
            Console.ReadLine();
        }
    }
}
