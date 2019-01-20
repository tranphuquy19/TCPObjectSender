using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPObjectServer;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ObjectClient objClient = new ObjectClient("127.0.0.1", 16059, true);
                objClient.Init();
                while (true)
                {
                    while (objClient.ReceiveObject().Count != 0)
                    {
                        Console.WriteLine((String)objClient.ReceiveObject().Dequeue());
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
