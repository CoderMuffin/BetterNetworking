using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using BetterAsync;
using System.Runtime.Serialization.Formatters.Binary;

namespace BetterNetworking
{
    public class WebChannel
    {
        private BinaryFormatter bf = new BinaryFormatter();
        public Socket socket;
        private NetworkStream stream;
        public int trafficCount
        {
            get => items.Count;
        }
        public WebChannel(IPEndPoint destination)
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            stream = new NetworkStream(socket);
            Flow.Go(() => {
                socket.Connect(destination);
            });
        }
        public void Send<T>(T data)
        {
            bf.Serialize(stream,data);
        }
        public Promise<T> Receive<T>()
        {
            return new Promise<T>((resolve, reject) =>
            {

            });
        }
    }
    public class WebReceiver
    {

    }
}
