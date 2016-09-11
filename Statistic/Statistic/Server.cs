using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Statistic
{
    public class Server
    {
        TcpListener server = new TcpListener(IPAddress.Any, 57033);
        List<TcpClient> clients = new List<TcpClient>();
        object locked = new object();
        bool runned = true;
        public Server()
        {
            server.Start();
            Run();
        }
        private void Run()
        {
            Thread th = new Thread(delegate()
            {
                while (runned)
                {
                    TcpClient lastClient = server.AcceptTcpClient();
                    lock (locked)
                    {
                        clients.Add(lastClient);
                        CheckConnect(lastClient);
                    }
                }
                lock (locked)
                {
                    foreach (var client in clients)
                        client.Close();
                }
            });
            th.IsBackground = true;
            th.Start();
        }
        private void CheckConnect(TcpClient client)
        {
            Thread th = new Thread(delegate ()
            {
                StreamReader sr=new StreamReader(client.GetStream());
                try
                {
                    sr.Read();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                finally
                {
                    lock (locked)
                    {
                        clients.Remove(client);
                    }
                }

            });
            th.IsBackground = true;
            th.Start();
        }
        public int GetCountClients()
        {
            return clients.Count;
        }
        public int GetCountClientsByIp(IPAddress ip)
        {
            int count = 0;
            foreach (var client in clients)
                if (((IPEndPoint)client.Client.RemoteEndPoint).Address.Equals(ip))
                    count++;
            return count;
        }
        public List<IPAddress> GetClientsIP()
        {
            List<IPAddress> list = new List<IPAddress>();
            foreach(var client in clients)
            {
                var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                if (!list.Contains(address))
                    list.Add(address);
            }
            return list;
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            res.Append("Clients in system now: " + clients.Count);
            foreach(var ip in GetClientsIP())
            {
                res.Append("\nCount clients from IP " + ip + ": " + GetCountClientsByIp(ip));
            }
            return res.ToString();
        }
    }
}
