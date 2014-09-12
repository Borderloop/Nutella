using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;


namespace ProductFeedReader
{
    public sealed class ProductQueue
    {
        public static Queue<Product> queue = new Queue<Product>();

        private static readonly object queueLock = new object();

        public static void Enqueue(Product p)
        {
            lock (queueLock)
            {              
                queue.Enqueue(p);
                Monitor.Pulse(queueLock);
            }
        }

        public static Product Dequeue()
        {
            lock (queueLock)
            {                
                while (queue.Count == 0)
                {
                    if (ProductFeedReader.isDone)
                    {
                        return null;
                    }
                    Monitor.Wait(queueLock, 2000);
                }               
                return queue.Dequeue();
            }
        }
    }
}
