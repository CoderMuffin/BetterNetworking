using System;
using System.Collections.Generic;
namespace BetterAsync
{
    /// <summary>
    /// A channel for coordinating between <see cref="Flow"/>s. 
    /// </summary>
    /// <typeparam name="T">The datatype of the <see cref="Channel{T}"/>.</typeparam>
    public class Channel<T>
    {
        private Queue<T> items;
        /// <summary>
        /// The number of items currently in the <see cref="Channel{T}"/>.
        /// </summary>
        public int trafficCount
        {
            get => items.Count;
        }
        /// <summary>
        /// Event that fires when an item is pushed to the <see cref="Channel{T}"/>.
        /// </summary>
        public event Action<T> OnReceived;
        /// <summary>
        /// Initializes a new <see cref="Channel{T}"/> for inter-flow communitation.
        /// </summary>
        public Channel()
        {
            items = new Queue<T>();
        }
        /// <summary>
        /// Push an item to the <see cref="Channel{T}"/>.
        /// </summary>
        /// <param name="data">The item to push</param>
        public void Send(T data)
        {
            items.Enqueue(data);
            OnReceived?.Invoke(data);
        }
        /// <summary>
        /// Read an item from the <see cref="Channel{T}"/>. If empty, wait for an item to be pushed.
        /// </summary>
        /// <returns>A <see cref="Promise{T}"/> awaiting the item</returns>
        public Promise<T> Receive()
        {
            return new Promise<T>((resolve, reject) =>
            {
                Flow.WaitUntil(() => items.Count != 0);
                resolve(items.Dequeue());
            });
        }
    }
}