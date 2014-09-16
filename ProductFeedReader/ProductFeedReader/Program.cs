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
        /// <summary>
        /// This thread is used by the productfeedreader, being the producer in this application.
        /// </summary>
        public static Thread producer;

        /// <summary>
        /// This thread is used by BOB, being the consumer in the application.
        /// </summary>
        public static Thread consumer;

        /// <summary>
        /// Main method will only start the ProductFeedReader
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Initialize
            Initialize();
            BOB bob = new BOB();
            bob.Process();
                      
            //Create threads
            //producer = new Thread(new ThreadStart(ProductFeedReader));
            //consumer = new Thread(new ThreadStart(ProductDequeuer));
            
            //Start threads
            //producer.Start();
            //consumer.Start();

            //Let the main thread wait for the other threads to finish
            while (producer.IsAlive && consumer.IsAlive) { }
            //Finalize and close the program.
            //FinalizeAndClose();
        }

        static void ProductDequeuer()
        {
            //Create BOB and start dequeueing items.
            BOB bob = new BOB();

            //Remain alive while thread one isnt done
            while (true)
            {
                //Take product from queue
                Product p = ProductQueue.Dequeue(); 

                //Break out of while loop when the queue gives null. This only happens when 
                //productfeedreader is flagged as done.
                if(p == null)
                {
                    break;
                }

                //Process the product otherwise.
                bob.Process(p);
            }
            Console.WriteLine("Thread 2 is done.");
        }

        static void ProductFeedReader()
        {
            //Create productfeedreader object.
            ProductFeedReader pfr = new ProductFeedReader();

            //Start it
            pfr.Start();
            Console.WriteLine("Thread 1 is done.");
        }

        static void Initialize()
        {
            //Initialize all the values for the static variables in the Statics class. These
            //variables are used throughout the whole program.
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            Statics.Logger = new Logger(Statics.settings["logpath"] + "\\log-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            Statics.TickCount = 0;
            Statics.TicksUntilSleep = Int32.Parse(Statics.settings["ticksuntilsleep"]);            
        }

        static void FinalizeAndClose()
        {
            //Close everything and shut down.
            Console.WriteLine("Writing data to logfile...");
            Statics.Logger.Close();
            Console.WriteLine("Done.");
            Environment.Exit(1);
        }

    }
}
