using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using BorderSource.Common;


namespace BobAndFriends
{
    public sealed class ProductQueue
    {
        /// <summary>
        /// This is the static singleton queue used for storing data.
        /// </summary>
        public static Queue<Product> queue = new Queue<Product>();

        /// <summary>
        /// The lock preventing both threads to access the queue at the same time.
        /// </summary>
        private static readonly object queueLock = new object();

        /// <summary>
        /// This method will put a product in the queue.
        /// </summary>
        /// <param name="p">The product to be enqueued.</param>
        public static void Enqueue(Product p)
        {
            //Take over the lock
            lock (queueLock)
            {              
                //Enqueue the product
                queue.Enqueue(p);

                //Signal the waiting object that it is done
                Monitor.Pulse(queueLock);
            }
        }

        /// <summary>
        /// This method will take a product out of the queue
        /// </summary>
        /// <returns></returns>
        public static Product Dequeue()
        {
            //Take over the lock
            lock (queueLock)
            {
                //If the queue is empty, keep on waiting.
                while (queue.Count == 0)
                {
                    //Make sure not to wait when the productfeedreader is flagged as done
                    if (ProductFeedReader.isDone)
                    {
                        //Stop waiting and return null.
                        return null;
                    }
                    Console.WriteLine("Queue starved, waiting...");
                    //Wait for the pulse, than release the lock
                    Monitor.Wait(queueLock, 2000);
                }  
             
                //Dequeue and return the product.
                return queue.Dequeue();
            }
        }
    }
}
