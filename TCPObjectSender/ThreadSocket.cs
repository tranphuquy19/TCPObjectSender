using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static TCPObjectSender.ObjectServer;

namespace TCPObjectSender
{
    public struct DataPackage
    {
        public string ipClient;
        public Queue<Object> queueData;
    }
    class ThreadSocket
    {
        public Object ObjToSend { get; set; }
        public Queue<Object> queueObjReceived { get; set; }
        public Socket socket { get; set; }

        private Thread send;
        private Thread receive;

        public ThreadSocket(Socket socket)
        {
            queueObjReceived = new Queue<Object>();
            this.socket = socket;
            send = new Thread(new ThreadStart(SendObject));
            send.Start();
            receive = new Thread(new ThreadStart(ReceiveObject));
            receive.Start();
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
                    this.queueObjReceived.Enqueue(Newtonsoft.Json.JsonConvert.DeserializeObject<Object>(objString));
                }
                catch (Exception)
                {
                    this.send.Abort();
                    streamReader.Close();
                    networkStream.Close();
                    //throw new Exception(this.socket.RemoteEndPoint.ToString());
                    ObjectServer.RemoveClient(this.socket.RemoteEndPoint.ToString());
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
                this.ObjToSend = null;
            }
            streamWriter.Close();
            networkStream.Close();
        }
    }
}
