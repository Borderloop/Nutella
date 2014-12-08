using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Misc
{
    public class SingletonQueue<T>
    {
        private static readonly object QueueLock = new object();

        private static readonly object InstanceLock = new object();

        private static SingletonQueue<T> _Instance;

        public static SingletonQueue<T> Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new SingletonQueue<T>();
                        }
                    }
                }
                return _Instance;
            }
        }

        private Queue<T> _Queue = new Queue<T>();
        public Queue<T> Queue
        {
            get
            {
                return _Queue;
            }
        }

        public bool InputStopped { get; set; }

        /// <summary>
        /// This method will put a product in the Queue.
        /// </summary>
        /// <param name="produit">The product to be enqueued.</param>
        public void Enqueue(T t)
        {
            lock (QueueLock)
            {
                Queue.Enqueue(t);
                Monitor.Pulse(QueueLock);
            }
        }

        /// <summary>
        /// This method will take a product out of the Queue
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            lock (QueueLock)
            {
                while (Queue.Count == 0)
                {
                    if (InputStopped)
                    {
                        return default(T);
                    }
                    Monitor.Wait(QueueLock, 2000);
                }
                return Queue.Dequeue();
            }
        }
    }
}
