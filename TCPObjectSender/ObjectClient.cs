using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCPObjectSender;

namespace TCPObjectServer
{
    public class ObjectClient
    {
        public Object Obj { get; set; }

        public string ipServer { get; set; }
        public int port { get; set; }

        private Socket socket;
        private IPEndPoint iep;
        private bool debug;
        private ThreadSocket threadSocket;

        public ObjectClient() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipServer"></param>
        /// <param name="port"></param>
        /// <param name="debug"></param>
        public ObjectClient(string ipServer, int port, bool debug = false)
        {
            this.ipServer = ipServer;
            this.port = port;
            this.debug = debug;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.iep = new IPEndPoint(IPAddress.Parse(this.ipServer), this.port);
            this.socket.Connect(this.iep);
            if (this.debug == true)
                Console.WriteLine(this.socket.LocalEndPoint.ToString() + " is connected!");
            threadSocket = new ThreadSocket(this.socket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Queue<Object> ReceiveObject()
        {
            return threadSocket.queueObjReceived;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void SendObject(Object obj)
        {
            if(obj != null)
            {
                threadSocket.ObjToSend = obj;
            }
            else
            {
                throw new InvalidOperationException("Object is null");
            }
        }
    }
}
