using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterNetworking
{
    class Program
    {
        static void Main(string[] args)
        {
            WebChannel rc = new WebChannel("127.0.0.1",7778);
            WebReceiver<string> webReceiver = new WebReceiver<string>(rc);
            webReceiver.OnReceive += Received;

            while (true)
            {
                Console.ReadLine();
                rc.Send("Hello World!");
            }
        }

        private static void Received(string obj)
        {
            Console.WriteLine("Receive: '" + obj + "'");
        }
    }
}
