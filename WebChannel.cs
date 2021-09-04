using System;
using System.Collections.Generic;
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
        private Queue<object> items=new Queue<object>();
        public WebChannel(string ip,int port=7777,int timeout=5000)
        {
            
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            
            Flow.Go(() => {
                socket.BeginConnect(IPAddress.Parse(ip),port,null,null).AsyncWaitHandle.WaitOne(timeout,true);
                if (!socket.Connected)
                {
                    socket.Close();
                    throw new InvalidOperationException("Failed to open connection");
                }
                
                stream = new NetworkStream(socket);
                while (true)
                {
                    items.Enqueue(bf.Deserialize(stream));
                }
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
                Flow.WaitUntil(() => items.Count <= 0);
                resolve((T)items.Dequeue());
            });
        }
    }
    public class WebReceiver<T>
    {
        public event Action<T> OnReceive;
        public bool receiving;
        public WebReceiver(WebChannel from)
        {
            Flow.Go(async () =>
            {
                while (true)
                {
                    try
                    {
                        Flow.WaitUntil(() => receiving);
                        OnReceive?.Invoke(await from.Receive<T>());
                        Flow.Wait(10);
                    }
                    catch (InvalidCastException) { } //not desired type
                }
            });
        }
    }
}
