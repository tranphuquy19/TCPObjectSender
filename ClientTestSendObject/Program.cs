using ServerTestSendObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPObjectSender;

namespace ClientTestSendObject
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ObjectClient<Animals> objClient = new ObjectClient<Animals>("127.0.0.1", 16059, true);
                objClient.Init();
                while (true)
                {
                    while (objClient.ReceiveObject().Count != 0)
                    {
                        Animals animals = (Animals)objClient.ReceiveObject().Dequeue();
                        //Console.WriteLine(animals);
                        Console.WriteLine("name: " + animals.name + " height: " + animals.height + " sex: " + animals.sex);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
