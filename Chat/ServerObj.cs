using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace Chat
{
    public class ServerObj
    {
        static TcpListener tcpListener;
        List<ClientObj> clients = new List<ClientObj>();
        protected internal void AddConnection(ClientObj clientObj)
        {
            clients.Add(clientObj);
        }
        protected internal void RemoveConection(string id)
        {
            ClientObj client = clients.FirstOrDefault(c => c.id == id);
            if (client != null)
            {
                clients.Remove(client);
            }
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server starts. Waiting for connect...");
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObj clientObj = new ClientObj(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObj.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].id != id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }
        protected internal void Disconnect()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
