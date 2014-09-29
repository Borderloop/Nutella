using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace BobAndFriends
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
        [STAThread]
        static void Main(string[] args)
        {
            
            //Initialize
            Initialize();
             
            //DatabaseJanitor crapper = new DatabaseJanitor();
            //crapper.Cleanup();    
  
            //Create threads
            producer = new Thread(new ThreadStart(ProductFeedReader));
            //consumer = new Thread(new ThreadStart(ProductDequeuer));
            
            //Start threads
            producer.Start();
            //consumer.Start();
            
            
            //Application.EnableVisualStyles();
            //Application.Run(new VisualBob());

            //sw = new Stopwatch();
             
            //Application.EnableVisualStyles();
            /*
            try
            {
                Application.Run(new VisualBob());
            } 
            catch(Exception e)
            {
                //don't do anything as it is not that important
            }
            finally
            {
                Application.Exit();
            }*/
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

            //Rerun all the products in the residue. We do not need ProductFeedReader for this.
            bob.RerunResidue();

            //Tidy up and close
            bob.FinalizeAndClose();
            foreach (string category in Statics.Logger._cats){
                Statics.Logger.WriteLine(category);
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
            Console.WriteLine("Queue size: " + ProductQueue.queue.Count);
        }

        static void Initialize()
        {
            //Initialize all the values for the static variables in the Statics class. These
            //variables are used throughout the whole program.
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            Statics.Logger = new Logger(Statics.settings["logpath"] + "\\log-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            Statics.TickCount = 0;
            Statics.TicksUntilSleep = Int32.Parse(Statics.settings["ticksuntilsleep"]);
            Statics.maxQueueSize = Int32.Parse(Statics.settings["maxqueuesize"]);
        }


    }
}
