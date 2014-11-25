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
using System.Data;
using System.Data.Entity.Validation;
using System.Runtime.InteropServices;
using BorderSource.Common;
using BorderSource.Queue;
using BobAndFriends.BobAndFriends;

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
        public static Thread validator;

        public static Thread consumer;

        public static DateTime StartTime;

        public static bool Done = false;

        public const int MAX_THREADS = 400;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Done = true;
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            TimeStatisticsMapper.Instance.StopAll();

            using (Logger logger = new Logger(Statics.LoggerPath, true))
            {
                logger.WriteStatistics();
            }
         
            producer.Abort();
            validator.Abort();
            consumer.Abort();

            Console.WriteLine("Shutting down.");

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion



        /// <summary>
        /// Main method will only start the ProductFeedReader
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            StartTime = DateTime.Now;
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //Initialize
            Initialize();

            //DatabaseJanitor crapper = new DatabaseJanitor();
            //crapper.Cleanup();    

            //TimeStatisticsMapper.Instance.StartTimeMeasure("Total time");

            //Create threads
            producer = new Thread(new ThreadStart(ProductFeedReader));
            validator = new Thread(new ThreadStart(ProductDequeuer));
            consumer = new Thread(new ThreadStart(StartBobBoxManager));

            //producer.Priority = ThreadPriority.BelowNormal;
            //consumer.Priority = ThreadPriority.BelowNormal;

            //Start threads
            producer.Start();
            validator.Start();
            consumer.Start();

            /*
            while(!Done)
            {
                Console.Clear();
                Console.WriteLine("Amount of products read by Reader: " + GeneralStatisticsMapper.Instance.Get("Products read"));
                Console.WriteLine("Amount of products validated by Bob: " + GeneralStatisticsMapper.Instance.Get("Products validated"));
                Console.WriteLine("Amount of products processed by BobBoxManager: " + GeneralStatisticsMapper.Instance.Get("Products processed"));
                Console.WriteLine("Current ProductQueue size: " + PackageQueue.Instance.Queue.Count);
                Console.WriteLine("Current ProductValidationQueue size: " + ProductValidationQueue.Instance.Queue.Count);
                Thread.Sleep(50);
            }*/
        }

        static void ProductDequeuer()
        {
          
            Package p = PackageQueue.Instance.Dequeue();
            List<Package> Packages = new List<Package>();
            DateTime StartRunning = new DateTime();
            List<Action> actions = new List<Action>();
            int count = 0;

            while (p != null)
            {
                count++;
                
                StartRunning = DateTime.Now;     
                while(Packages.Count < MAX_THREADS)
                {
                    Packages.Add(p);
                    GeneralStatisticsMapper.Instance.Increment("Total amount of products processed", p.products.Count);
                    p = PackageQueue.Instance.Dequeue();
                    if (p == null) break;                   
                }

                Console.WriteLine("Started processing group " + count + " consisting of " + Packages.Count + " packages");

                for (int i = 0; i < Packages.Count; i++)
                {
                    int copy = i;
                    Package package = Packages[i];
                    actions.Add(new Action(() => StartAnotherBob(package)));
                }
                 
                
                //TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent reading packages from " + WebshopPackages.First().Webshop);
                Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MAX_THREADS }, actions.ToArray());
                //TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent reading packages from " + WebshopPackages.First().Webshop);
                
                Console.WriteLine("Time spent reading packages from group " + count + ": " + (DateTime.Now - StartRunning));
                Console.WriteLine("Total time since start: " + (DateTime.Now - StartTime));
              
                Packages.Clear();
                actions.Clear();
            }
            ProductValidationQueue.Instance.InputStopped = true;

            //Rerun all the products in the residue. We do not need ProductFeedReader for this.
            //bob.RerunResidue();

            //Console.WriteLine("Validation is done.");
            //Console.WriteLine("ProductValidationQueue size: " + ProductValidationQueue.Instance.Queue.Count);
            
        }

        static void StartAnotherBob(Package p)
        {
            BOB bob = new BOB();
            try
            {
                bob.Process(p);
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Caught OutOfMemoryException, trying GC.Collect()");
                GC.Collect();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("duplicate"))
                    Console.WriteLine("Threw a duplicate error.");
                else Console.WriteLine(e.ToString());
            }
        }

        static void ProductFeedReader()
        {
            //Create productfeedreader object.
            ProductFeedReader pfr = new ProductFeedReader();

            //Start it
            //TimeStatisticsMapper.Instance.StartTimeMeasure("Time reading products");
            pfr.Start();
            //TimeStatisticsMapper.Instance.StopTimeMeasure("Time reading products");

            //Console.WriteLine("Done producing.");
            //Console.WriteLine("PackageQueue size: " + PackageQueue.Instance.Queue.Count);
        }

        static void StartBobBoxManager()
        {
            BobboxManager bbm = new BobboxManager();
            TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent validating and saving");
            bbm.StartValidatingAndSaving();
            TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent validating and saving");
            //Console.WriteLine("Done consuming.");
            TimeStatisticsMapper.Instance.StopTimeMeasure("Total time");

            using (Logger logger = new Logger(Statics.LoggerPath))
            {
                logger.WriteStatistics();
            }

            TimeStatisticsMapper.Instance.StopTimeMeasure("Total time");
            Done = true;
            Console.WriteLine("Press ENTER to exit.");
            Console.Read();
        }

        static void Initialize()
        {
            //Initialize all the values for the static variables in the Statics class. These
            //variables are used throughout the whole program.

            #region Settings
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion

            #region Loggers
            Statics.LoggerPath = Statics.settings["logpath"] + "\\log-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            Statics.SqlLogger = new QueryLogger(Statics.settings["logpath"] + "\\querydump" + DateTime.Now.ToString("MMddHHmm") + ".txt");
            Statics.AmiguousLogPath = Statics.settings["logpath"] + "\\ambiguous-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            #endregion

            #region Values
            Statics.TickCount = 0;
            Statics.TicksUntilSleep = Int32.Parse(Statics.settings["ticksuntilsleep"]);
            Statics.maxQueueSize = Int32.Parse(Statics.settings["maxqueuesize"]);
            Statics.maxSkuSize = Int32.Parse(Statics.settings["maxskusize"]);
            Statics.maxBrandSize = Int32.Parse(Statics.settings["maxbrandsize"]);
            Statics.maxTitleSize = Int32.Parse(Statics.settings["maxtitlesize"]);
            Statics.maxImageUrlSize = Int32.Parse(Statics.settings["maximageurlsize"]);
            Statics.maxCategorySize = Int32.Parse(Statics.settings["maxcategorysize"]);
            Statics.maxShipTimeSize = Int32.Parse(Statics.settings["maxshiptimesize"]);
            Statics.maxWebShopUrlSize = Int32.Parse(Statics.settings["maxwebshopurlsize"]);
            Statics.maxDirectLinkSize = Int32.Parse(Statics.settings["maxdirectlinksize"]);
            Statics.maxAffiliateNameSize = Int32.Parse(Statics.settings["maxaffiliatenamesize"]);
            Statics.maxAffiliateProductIdSize = Int32.Parse(Statics.settings["maxaffiliateproductidsize"]);
            Statics.maxResidueListSize = Int32.Parse(Statics.settings["maxresiduelistsize"]);
            #endregion

            #region Mapping of DBProduct to BobProduct
            Statics.TwoWayDBProductToBobProductMapping = new Dictionary<string, string>();
            Statics.TwoWayDBProductToBobProductMapping.Add("ship_time", "DeliveryTime");
            Statics.TwoWayDBProductToBobProductMapping.Add("ship_cost", "DeliveryCost");
            Statics.TwoWayDBProductToBobProductMapping.Add("price", "Price");
            Statics.TwoWayDBProductToBobProductMapping.Add("webshop_url", "Webshop");
            Statics.TwoWayDBProductToBobProductMapping.Add("direct_link", "Url");
            Statics.TwoWayDBProductToBobProductMapping.Add("last_modified", "LastModified");
            Statics.TwoWayDBProductToBobProductMapping.Add("valid_until", "ValidUntil");
            Statics.TwoWayDBProductToBobProductMapping.Add("affiliate_name", "Affiliate");
            Statics.TwoWayDBProductToBobProductMapping.Add("affiliate_unique_id", "AffiliateProdID");
            #endregion

            #region Mapping of BobProduct to DBProduct
            Statics.TwoWayDBProductToBobProductMapping.Add("DeliveryTime", "ship_time");
            Statics.TwoWayDBProductToBobProductMapping.Add("DeliveryCost", "ship_cost");
            Statics.TwoWayDBProductToBobProductMapping.Add("Price", "price");
            Statics.TwoWayDBProductToBobProductMapping.Add("Webshop", "webshop_url");
            Statics.TwoWayDBProductToBobProductMapping.Add("Url", "direct_link");
            Statics.TwoWayDBProductToBobProductMapping.Add("LastModified", "last_modified");
            Statics.TwoWayDBProductToBobProductMapping.Add("ValidUntil", "valid_until");
            Statics.TwoWayDBProductToBobProductMapping.Add("Affiliate", "affiliate_name");
            Statics.TwoWayDBProductToBobProductMapping.Add("AffiliateProdID", "affiliate_unique_id");
            #endregion

            BetsyDbContextReader reader = new BetsyDbContextReader();
            Lookup.WebshopLookup = reader.GetAllWebshops();
            Lookup.CategoryLookup = reader.GetAllCategories();

            TimeStatisticsMapper.Instance.StartTimeMeasure("Total time");
        }
    }
}
