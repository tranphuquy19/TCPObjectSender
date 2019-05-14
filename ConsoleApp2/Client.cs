using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPObjectSender;

namespace ConsoleApp2
{
    class Client
    {
        public static Thread send;
        public static Thread receive;

        public static ObjectClient<String> objClient = new ObjectClient<String>("127.0.0.1", 16059, true);


        public static void Send()
        {
            while (true)
            {
                string st = Console.ReadLine();
                if (st.Equals("quit")) break;
                objClient.SendObject(st);

            }
        }

        public static void Receive()
        {
            while (true)
            {
                while (objClient.ReceiveObject().Count != 0)
                {
                    Console.WriteLine((String)objClient.ReceiveObject().Dequeue());
                }
            }
        }

        static void Main(string[] args)
        {
            objClient.Init();
            send = new Thread(new ThreadStart(Send));
            receive = new Thread(new ThreadStart(Receive));
            send.Start();
            receive.Start();
            Console.ReadLine();
        }
    }
}
