using System;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class ClientObj
    {
        protected internal string id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObj server;
        public ClientObj(TcpClient tcpClient, ServerObj serverObj)
        {
            id = Guid.NewGuid().ToString();
            this.client = tcpClient;
            this.server = serverObj;
            server.AddConnection(this);
        }
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                this.userName = message;

                message = userName + " connect";
                server.BroadcastMessage(message, this.id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.id);
                    }
                    catch
                    {
                        message = String.Format("{0} left.", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.id);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveConection(this.id);
                Close();
            }
        }
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);
            return builder.ToString();
        }
        protected internal void Close()
        {
            if(Stream != null)
            {
                Stream.Close();
            }
            if(client != null)
            {
                client.Close();
            }
        }

    }
}
