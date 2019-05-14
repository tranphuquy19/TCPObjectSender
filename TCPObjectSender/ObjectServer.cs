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
    public class ObjectServer<T>
    {


        public T Obj { get; set; }

        public int port { get; set; }
        public int limitClient { get; set; }

        public Hashtable listClient = new Hashtable();

        private Socket socket;
        private IPEndPoint iep;
        private static bool debug;

        ThreadSocket<T> threadDestroy = new ThreadSocket<T>();

        private Thread threadInit;

        public ObjectServer()
        {
            threadDestroy.isDestroy = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">port of Server</param>
        /// <param name="limitClient">Maximun Client, default is 16 clients at once</param>
        /// <param name="_debug">Show debug message on Console</param>
        public ObjectServer(int port, int limitClient = 16, bool _debug = false)
        {
            threadDestroy.isDestroy = true;
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
                listClient.Add(clientSocket.RemoteEndPoint.ToString(), new ThreadSocket<T>(clientSocket, this));
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

        /// <summary>
        /// Override this method
        /// </summary>
        /// <param name="obj">Data of Obj</param>
        /// <param name="ipClient">ip Address of target client</param>
        public virtual void CustomSend(T obj, string ipClient)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void SendObjectToAll(T obj)
        {
            foreach (ThreadSocket<T> client in listClient.Values)
            {
                if (debug == true && client.isDestroy == false)
                    Console.WriteLine(client.socket.RemoteEndPoint.ToString() + " was send!");
                client.ObjToSend = obj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ipClient"></param>
        public void SendObjectToIP(T obj, string ipClient)
        {
            if (listClient.ContainsKey(ipClient))
            {
                ThreadSocket<T> client = (ThreadSocket<T>)listClient[ipClient];
                client.ObjToSend = obj;
                listClient[ipClient] = client;
            }
            else
                throw new InvalidOperationException("IP of client not found");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Queue<DataPackage<T>> ReceiveObject()
        {
            DataPackage<T> package = new DataPackage<T>();
            Queue<DataPackage<T>> list = new Queue<DataPackage<T>>();
            #region Debug
            //try
            //{
            //    foreach (ThreadSocket<T> client in listClient.Values)
            //    {
            //        if(client == null)
            //        {
            //            listClient.Remove(listClient[client.socket.RemoteEndPoint.ToString()]);
            //        }
            //        package = new DataPackage<T>();
            //        if (client.queueObjReceived != null)
            //        {
            //            package.ipClient = client.socket.RemoteEndPoint.ToString();
            //            package.queueData = client.queueObjReceived;
            //        }
            //        list.Enqueue(package);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("from ObjectServer.ReceiveObject()"+e.Message);
            //    return list;
            //}

            #endregion
            
            foreach (ThreadSocket<T> client in listClient.Values)
            {
                if (client.isDestroy)
                    continue;
                package = new DataPackage<T>();
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
        public void RemoveClient(string ipClient)
        {
            if (debug == true)
            {
                Console.WriteLine(ipClient + " has disconnected!");
            }
            listClient[ipClient] = threadDestroy;
        }
    }
}
