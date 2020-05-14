using System;
using System.Threading;

namespace Chat
{
    class Program
    {
        static ServerObj server;
        static Thread threadListen;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObj();
                threadListen = new Thread(new ThreadStart(server.Listen));
                threadListen.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
