using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace BetterAsync
{
    /// <summary>
    /// The exception thrown when a <see cref="Promise{T}"/> is rejected.
    /// </summary>
    [Serializable]
    public class PromiseRejectionException : Exception
    {
        public PromiseRejectionException() { }
        public PromiseRejectionException(string message) : base(message) { }
        public PromiseRejectionException(string message, Exception inner) : base(message, inner) { }
        protected PromiseRejectionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// Represents a promise that can be awaited. When resolve or reject is called, the <see cref="Promise{T}"/> stops awaiting.
    /// </summary>
    /// <typeparam name="T">The type that the <see cref="Promise{T}"/> resolves to.</typeparam>
    public class Promise<T>
    {
        public event Action<T> OnResolved;
        TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
        Flow actionFlow;
        /// <summary>
        /// Creates a new <see cref="Promise{T}"/>.
        /// </summary>
        /// <param name="callback">The function the <see cref="Promise{T}"/> runs. Evaluates in its own <see cref="Flow"/>.</param>
        public Promise(Action<Action<T>, Action> callback)
        {
            actionFlow = Flow.Go(() => {
                callback(Resolve, Reject);
            });
            while (!tcs.Task.IsCompleted) { }
        }
        /// <summary>
        /// Rejects the <see cref="Promise{T}"/>. Thows a <see cref="PromiseRejectionException"/> where it is called.
        /// </summary>
        public void Reject()
        {
            actionFlow.Stop();
            throw new PromiseRejectionException();
        }
        /// <summary>
        /// Resolves the <see cref="Promise{T}"/>. The <see cref="Promise{T}"/> will stop awaiting and return the value.
        /// </summary>
        /// <param name="result">The value to return.</param>
        public void Resolve(T result)
        {
            OnResolved?.Invoke(result);
            actionFlow.Stop();
            tcs.SetResult(result);
        }
        /// <summary>
        /// Callback when the <see cref="Promise{T}"/> is resolved.
        /// </summary>
        /// <param name="callback">The method to call.</param>
        public void Then(Action<T> callback)
        {
            OnResolved += callback;
        }

        /// <summary>
        /// The <see cref="TaskAwaiter{TResult}"/> for the <see cref="Promise{T}"/>.
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<T> GetAwaiter()
        {
            return tcs.Task.GetAwaiter();
        }
    }
}