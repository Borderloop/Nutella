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
using BorderSource.ProductAssociation;
using BorderSource.Statistics;
using BorderSource.Loggers;

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

        public const int MAX_BOBS = 1;

        public const int MAX_BOBBOXMANAGERS = 3;

        public const int MAX_READERS = 3;

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
        /// Main method will only start the FeedReaderController
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
            producer = new Thread(new ThreadStart(FeedReaderController));
            validator = new Thread(new ThreadStart(BobController));
            consumer = new Thread(new ThreadStart(StartBobBoxManager));

            //Start threads
            producer.Start();
            validator.Start();
            consumer.Start();
        }

        static void BobController()
        {
            Console.WriteLine("Started BOB.");

            List<Package> Packages = new List<Package>();
            DateTime StartRunning = new DateTime();
            List<Action> actions = new List<Action>();
            int count = 0;

            TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent validating");
            while (true)
            {
                //Break if no packages are found
                if ((Packages = PackageQueue.Instance.DequeuePackageByAmount(MAX_BOBS)) == null) break;

                count++;
                StartRunning = DateTime.Now;                                       
                int totalAmount = Packages.Sum(pack => pack.products.Count);

                Console.WriteLine("Started processing group " + count + " consisting of " + Packages.Count + " packages and " + totalAmount + " products");               

                for (int i = 0; i < Packages.Count; i++)
                {
                    int copy = i;
                    actions.Add(new Action(() => StartAnotherBob(Packages[copy], copy)));
                }

                Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MAX_BOBS }, actions.ToArray());

                GeneralStatisticsMapper.Instance.Increment("Total amount of products processed", totalAmount);

                Console.WriteLine("Time spent reading packages from group " + count + ": " + (DateTime.Now - StartRunning));
                Packages.Clear();
                actions.Clear();
            }
            TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent validating");
            ProductValidationQueue.Instance.InputStopped = true;

            Console.WriteLine("Validation is done.");
        }

        static void StartAnotherBob(Package p, int id)
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
            //catch (Exception e)
            //{ 
            //    Console.WriteLine("A Bob threw an error: " + e.Message);
            //    Console.WriteLine("Continuing with another Bob.");
            //}
            finally
            {
                bob.Dispose();
            }
        }

        static void FeedReaderController()
        {
            //Create productfeedreader object.
            ProductFeedReader pfr = new ProductFeedReader();

            TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent reading");
            pfr.Start(MAX_READERS);
            TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent reading");
        }

        static void BobBoxManagerController()
        {
            List<Action> managers = new List<Action>();
            for(int i = 0; i < MAX_BOBBOXMANAGERS; i++)
            {
                managers.Add(new Action(() => StartBobBoxManager()));
            }

            TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent saving");
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MAX_BOBBOXMANAGERS }, managers.ToArray());
            TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent saving");

            Console.WriteLine("Done consuming.");

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

        static void StartBobBoxManager()
        {
            Console.WriteLine("Started a BobBoxManager.");

            BobboxManager bbm = new BobboxManager();          
            bbm.StartValidatingAndSaving();                     
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
            Statics.maxSizes = new Dictionary<string, int>();
            Statics.maxSizes.Add("max_queue_size", Int32.Parse(Statics.settings["maxqueuesize"]));
            Statics.maxSizes.Add("max_sku_size", Int32.Parse(Statics.settings["maxskusize"]));
            Statics.maxSizes.Add("max_brand_size", Int32.Parse(Statics.settings["maxbrandsize"]));
            Statics.maxSizes.Add("max_title_size", Int32.Parse(Statics.settings["maxtitlesize"]));
            Statics.maxSizes.Add("max_imageurl_size", Int32.Parse(Statics.settings["maximageurlsize"]));
            Statics.maxSizes.Add("max_category_size", Int32.Parse(Statics.settings["maxcategorysize"]));
            Statics.maxSizes.Add("max_shiptime_size", Int32.Parse(Statics.settings["maxshiptimesize"]));
            Statics.maxSizes.Add("max_webshopurl_size", Int32.Parse(Statics.settings["maxwebshopurlsize"]));
            Statics.maxSizes.Add("max_directlink_size", Int32.Parse(Statics.settings["maxdirectlinksize"]));
            Statics.maxSizes.Add("max_affiliatename_size", Int32.Parse(Statics.settings["maxaffiliatenamesize"]));
            Statics.maxSizes.Add("max_affiliateproductid_size", Int32.Parse(Statics.settings["maxaffiliateproductidsize"]));
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
