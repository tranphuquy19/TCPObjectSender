using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace TCPObjectSender
{
    public class ObjectServer
    {
        

        public Object Obj { get; set; }

        public int port { get; set; }
        public int limitClient { get; set; }

        static protected Hashtable listClient = new Hashtable();

        private Socket socket;
        private IPEndPoint iep;
        private static bool debug;

        private Thread threadInit;

        public ObjectServer() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ipClient"></param>
        /// <param name="port"></param>
        /// <param name="limitClient"></param>
        public ObjectServer(int port, int limitClient = 16, bool _debug = false)
        {
            this.port = port;
            this.limitClient = limitClient;
            debug = _debug;
        }

        private void GetHandShakes()
        {
            while (true)
            {
                Socket clientSocket = this.socket.Accept();
                if (debug == true)
                    Console.WriteLine(clientSocket.RemoteEndPoint.ToString() + " is connected!");
                listClient.Add(clientSocket.RemoteEndPoint.ToString(), new ThreadSocket(clientSocket));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.iep = new IPEndPoint(IPAddress.Any, this.port);
            socket.Bind(this.iep);
            socket.Listen(this.limitClient);
            threadInit = new Thread(new ThreadStart(GetHandShakes));
            threadInit.Start();
        }

        public virtual void CustomSend(Object obj, string ipClient)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void SendObjectToAll(Object obj)
        {
            foreach(ThreadSocket client in listClient.Values)
            {
                if (debug == true)
                    Console.WriteLine(client.socket.RemoteEndPoint.ToString() + " was send!");
                client.ObjToSend = obj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ipClient"></param>
        public void SendObjectToIP(Object obj, string ipClient)
        {
            foreach (ThreadSocket client in listClient)
            {
                string ip = client.socket.RemoteEndPoint.ToString();
                if (ip == ipClient)
                {
                    Console.WriteLine(ip + " was send!");
                    client.ObjToSend = obj;
                    return;
                }
            }
            throw new InvalidOperationException("IP of client not found");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Queue<DataPackage> ReceiveObject()
        {
            DataPackage package;
            Queue<DataPackage> list = new Queue<DataPackage>();
            foreach (ThreadSocket client in listClient)
            {
                package = new DataPackage();
                if (client.queueObjReceived != null)
                {
                    package.ipClient = client.socket.RemoteEndPoint.ToString();
                    package.queueData = client.queueObjReceived;
                }
                list.Enqueue(package);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipClient"></param>
        public static void RemoveClient(string ipClient)
        {
            if(debug == true)
            {
                Console.WriteLine(ipClient + " is disconnected!");
            }
            listClient.Remove(ipClient);
        }
    }
}
