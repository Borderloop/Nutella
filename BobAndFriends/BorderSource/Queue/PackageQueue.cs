using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using BorderSource.Common;
using BorderSource.ProductAssociation;


namespace BorderSource.Queue
{
    public class PackageQueue 
    {
        private static readonly object QueueLock = new object();

        private static readonly object InputLock = new object();

        private static readonly object InstanceLock = new object();

        private static PackageQueue _Instance;

        public static PackageQueue Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new PackageQueue();
                        }
                    }
                }
                return _Instance;
            }
        }

        private Queue<Package> _Queue = new Queue<Package>();
        public Queue<Package> Queue
        {
            get
            {
                return _Queue;
            }
        }

        private bool _InputStopped = false;
        public bool InputStopped
        {
            get
            {
                lock(InputLock)
                {
                    return _InputStopped;
                }
            }
            set
            {
                lock(InputLock)
                {
                    _InputStopped = value;
                }
            }
        }

        /// <summary>
        /// This method will put a product in the Queue.
        /// </summary>
        /// <param name="produit">The product to be enqueued.</param>
        public void Enqueue(Package t)
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
        public Package Dequeue()
        {
            lock (QueueLock)
            {
                while (Queue.Count == 0)
                {
                    if (InputStopped)
                    {
                        return null;
                    }
                    // Console.WriteLine("PackageQueue starved.");
                    Monitor.Wait(QueueLock, 3000);
                }
                return Queue.Dequeue();
            }
        }

        public List<Package> DequeuePackageByAmount(int amount)
        {
            List<Package> list = new List<Package>();
            lock(QueueLock)
            {
                while (Queue.Count == 0)
                {
                    if (InputStopped)
                    {
                        return null;
                    }
                    Monitor.Wait(QueueLock, 3000);
                }
                while (list.Count < amount && Queue.Count > 0)
                {
                    list.Add(Queue.Dequeue());
                }               
            }
            return list;
        }
    }
}
