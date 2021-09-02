using System;
using System.Threading;
namespace BetterAsync
{
    /// <summary>
    /// A simplified <see cref="Thread"/> implementation. This class is <see langword="partial"/>, so it can be statically extended.
    /// </summary>
    public partial class Flow //allow static extensions
    {
        /// <summary>
        /// The base <see cref="Thread"/> that the <see cref="Flow"/> masks.
        /// </summary>
        public Thread thread;
        /// <summary>
        /// Whether the <see cref="Flow"/> is currently active.
        /// </summary>
        public bool running;
        /// <summary>
        /// Creates a new <see cref="Flow"/>.
        /// </summary>
        /// <param name="todo">The <see cref="Action"/> that the <see cref="Flow"/> runs</param>
        public Flow(Action todo)
        {
            thread = new Thread(() => {
                try
                {
                    todo();
                }
                catch (ThreadAbortException) { }
                catch (ThreadInterruptedException) { }
            });
        }
        /// <summary>
        /// Starts the <see cref="Flow"/>.
        /// </summary>
        public void Start()
        {
            if (!running) running = true;
            else throw new InvalidOperationException("Flow is already running");
            thread.Start();
        }
        /// <summary>
        /// Creates and starts a new <see cref="Flow"/>.
        /// </summary>
        /// <param name="todo">The <see cref="Action"/> that the <see cref="Flow"/> runs.</param>
        /// <returns>The <see cref="Flow"/> created.</returns>
        public static Flow Go(Action todo)
        {
            Flow f = new Flow(todo);
            f.Start();
            return f;
        }
        /// <summary>
        /// Pause the current <see cref="Flow"/> until the condition evaluates to <see langword="true"/>.
        /// </summary>
        /// <param name="until">The condition to evaluate.</param>
        public static void WaitUntil(Func<bool> until)
        {
            while (!until()) { Wait(10); }
        }
        /// <summary>
        /// Pause the current <see cref="Flow"/> until the condition evaluates to <see langword="true"/>.
        /// </summary>
        /// <param name="until">The condition to evaluate.</param>
        /// <param name="interval">The interval between checking the condition.</param>
        public static void WaitUntil(Func<bool> until, uint interval)
        {
            while (!until()) { Wait(interval); }
        }
        /// <summary>
        /// Pause the current <see cref="Flow"/> until the condition evaluates to <see langword="false"/>.
        /// </summary>
        /// <param name="until">The condition to evaluate.</param>
        /// <param name="interval">The interval between checking the condition.</param>
        public static void WaitWhile(Func<bool> until, uint interval)
        {
            while (!until()) { Wait(interval); }
        }
        /// <summary>
        /// Pause the current <see cref="Flow"/> until the condition evaluates to <see langword="false"/>.
        /// </summary>
        /// <param name="until">The condition to evaluate.</param>
        public static void WaitWhile(Func<bool> until)
        {
            while (!until()) { Wait(10); }
        }
        /// <summary>
        /// Pause the <see cref="Flow"/> for the specified number of milliseconds.
        /// </summary>
        /// <param name="ms">The number of milliseconds the <see cref="Flow"/> pauses for.</param>
        public static void Wait(uint ms)
        {
            Thread.Sleep((int)ms);
        }
        /// <summary>
        /// Stop the <see cref="Flow"/>.
        /// </summary>
        public void Stop()
        {
            if (running) running = false;
            else throw new InvalidOperationException("Flow is not running");
            try
            {
                thread.Abort();
            }
            catch (PlatformNotSupportedException)
            {
                thread.Interrupt();
            }
        }
    }
}