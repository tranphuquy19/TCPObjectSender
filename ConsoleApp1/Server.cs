using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPObjectSender;

namespace ConsoleApp1
{
    class Server
    {
        public static Thread send;
        public static Thread receive;
        static ObjectServer<String> objectServer = new ObjectServer<String>(16059, 16, true);

        public static void Send()
        {
            while (true)
            {
                string st = Console.ReadLine();
                if (st.Equals("quit")) break;
                objectServer.SendObjectToAll(st);

            }
        }

        public static void Receive()
        {
            while (true)
            {
                while (objectServer.ReceiveObject().Count > 0)
                {
                    DataPackage<String> dataPackage = objectServer.ReceiveObject().Dequeue();
                    while (dataPackage.queueData.Count > 0)
                    {
                        Console.WriteLine(dataPackage.ipClient + " was send: " + (String)dataPackage.queueData.Dequeue());
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            objectServer.Init();
            send = new Thread(new ThreadStart(Send));
            receive = new Thread(new ThreadStart(Receive));
            send.Start();
            receive.Start();
            Console.ReadLine();

        }
    }
}
