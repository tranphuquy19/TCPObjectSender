using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPObjectSender;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ObjectServer objectServer = new ObjectServer(16059, 16, true);
                objectServer.Init();
                while (true)
                {
                    string st = Console.ReadLine();
                    if (st.Equals("quit")) break;
                    objectServer.SendObjectToAll(st);
                }
                Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
