using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TCPObjectSender
{
    public struct DataPackage<T>
    {
        public string ipClient { get; set; }
        public Queue<T> queueData { get; set; }
    }
    class ThreadSocket<T>
    {
        public T ObjToSend { get; set; }
        public Queue<T> queueObjReceived { get; set; }
        public Socket socket { get; set; }
        public ObjectServer<T> objectServer { get; set; }
        public bool isDestroy { get; set; }

        private Thread send;
        private Thread receive;

        public ThreadSocket(Socket socket, ObjectServer<T> objectServer)
        {
            this.objectServer = objectServer;
            queueObjReceived = new Queue<T>();
            this.socket = socket;
            send = new Thread(new ThreadStart(SendObject));
            send.Start();
            receive = new Thread(new ThreadStart(ReceiveObject));
            receive.Start();
        }

        public ThreadSocket(Socket socket)
        {
            queueObjReceived = new Queue<T>();
            this.socket = socket;
            send = new Thread(new ThreadStart(SendObject));
            send.Start();
            receive = new Thread(new ThreadStart(ReceiveObject));
            receive.Start();
        }
        public ThreadSocket()
        {

            this.isDestroy = false;
        }
        private void ReceiveObject()
        {
            NetworkStream networkStream = new NetworkStream(this.socket);
            StreamReader streamReader = new StreamReader(networkStream, System.Text.Encoding.Unicode, true);
            while (true)
            {
                String objString = "";
                try
                {
                    objString = streamReader.ReadLine();
                    this.queueObjReceived.Enqueue(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objString));
                }
                catch (Exception)
                {
                    this.send.Abort();
                    streamReader.Close();
                    networkStream.Close();
                    //throw new Exception(this.socket.RemoteEndPoint.ToString());
                    objectServer.RemoveClient(this.socket.RemoteEndPoint.ToString());
                    break;
                }
            }

        }

        private void SendObject()
        {
            NetworkStream networkStream = new NetworkStream(this.socket);
            StreamWriter streamWriter = new StreamWriter(networkStream, System.Text.Encoding.Unicode);
            while (true)
            {
                if (this.ObjToSend == null) continue;
                string objString = Newtonsoft.Json.JsonConvert.SerializeObject(this.ObjToSend);
                try
                {
                    streamWriter.WriteLine(objString);
                }
                catch (Exception)
                {
                    break;
                }
                streamWriter.Flush();
                this.ObjToSend = default(T);
            }
            streamWriter.Close();
            networkStream.Close();
        }
    }
}
