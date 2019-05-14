using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPObjectSender;

namespace ServerTestSendObject
{
    public class Animals
    {
        public string name { get; set; }
        public int height { get; set; }
        public bool sex { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ObjectServer<Animals> objectServer = new ObjectServer<Animals>(16059, 16, true);
                objectServer.Init();
                while (true)
                {
                    Animals animals = new Animals();
                    animals.name = "dog";
                    animals.height = 100;
                    animals.sex = true;
                    objectServer.SendObjectToAll(animals);
                    Thread.Sleep(1500);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
