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
        public static Thread producer;
        public static Thread consumer;

        /// <summary>
        /// Main method will only start the ProductFeedReader
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Initialize
            Initialize();
                       
            //Create threads
            producer = new Thread(new ThreadStart(ProductFeedReader));
            consumer = new Thread(new ThreadStart(ProductDequeuer));
            
            //Start threads
            producer.Start();
            consumer.Start();

            //Let the main thread wait for the other threads to finish
            while (producer.IsAlive && consumer.IsAlive) { }
            //Finalize and close the program.
            FinalizeAndClose();
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
            Console.WriteLine("Writing data to logfile...");
            Statics.Logger.Close();
            Console.WriteLine("Done.");
            Environment.Exit(1);
        }

    }
}
