using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Threading;

namespace ProductFeedReader
{
    class Program
    {
        public static Thread t1;
        public static Thread t2;
        /// <summary>
        /// Main method will only start the ProductFeedReader
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //ProductFeedReader pfr = new ProductFeedReader();
            //pfr.Start();
            BOB BOB = new BOB();
            BOB.Process();
            
            
            //Create threads
            //t1 = new Thread(new ThreadStart(ProductFeedReader));
            //t2 = new Thread(new ThreadStart(ProductDequeuer));
            
            //Start threads
            //t1.Start();
            //t2.Start();
        }

        static void ProductDequeuer()
        {
            //Create BOB and start dequeueing items.
            BOB bob = new BOB();
            while (true)
            {
                Product p = ProductQueue.Dequeue(); 
                if(p == null)
                {
                    break;
                }
                bob.Process(p);
            }
            Console.WriteLine("Thread 2 is done.");
        }

        static void ProductFeedReader()
        {
            //Exectue the ProductFeedReader.
            ProductFeedReader pfr = new ProductFeedReader();
            pfr.Start();
            Console.WriteLine("Thread 1 is done.");
        }

    }
}
