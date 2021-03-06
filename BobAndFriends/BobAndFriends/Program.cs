﻿using System;
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
using BobAndFriends.Global;

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

        public static int MAX_BOBS;

        public static int MAX_BOBBOXMANAGERS;

        public static int MAX_READERS;

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

            Logger.Instance.WriteStatistics();

            Thread.Sleep(500);

            producer.Abort();
            validator.Abort();
            consumer.Abort();

            Console.WriteLine("Shutting down.");

            // shutdown right away so there are no lingering threads
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

            Console.SetWindowPosition(0, 0);

            /*Console.WriteLine("Parsing data...");
            Crapper.Crapper.Parse();
            Console.WriteLine("Done parsing data.");*/

            // Initialize
            Initialize();

            // Create threads
            producer = new Thread(new ThreadStart(FeedReaderController));
            validator = new Thread(new ThreadStart(BobController));
            consumer = new Thread(new ThreadStart(BobBoxManagerController));
            
            // Start threads
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
                // Break if no packages are found
                if ((Packages = PackageQueue.Instance.DequeuePackageByAmount(Properties.PropertyList["packages_per_validation"].GetValue<int>())) == null) break;

                count++;
                StartRunning = DateTime.Now;                                       
                int totalAmount = Packages.Sum(pack => pack.products.Count);

                Console.WriteLine("Started processing group " + count + " consisting of " + Packages.Count + " packages and " + totalAmount + " products");               

                for (int i = 0; i < Packages.Count; i++)
                {
                    int copy = i;
                    actions.Add(new Action(() => StartAnotherBob(Packages[copy])));
                }

                Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MAX_BOBS }, actions.ToArray());

                GeneralStatisticsMapper.Instance.Increment("Total amount of products processed", totalAmount);
                GeneralStatisticsMapper.Instance.Increment("Total amount of packages");

                Console.WriteLine("Time spent reading packages from group " + count + ": " + (DateTime.Now - StartRunning));
                Packages.Clear();
                actions.Clear();
            }
            TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent validating");
            ProductValidationQueue.Instance.InputStopped = true;

            Console.WriteLine("Validation is done.");
        }

        static void StartAnotherBob(Package p)
        {
            using (var bob = new BOB())
            {
                bob.Process(p);
            }
        }

        static void FeedReaderController()
        {                       
            try
            {
                TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent reading");

                // Create productfeedreader object.
                ProductFeedReader pfr = new ProductFeedReader();
                pfr.Start(MAX_READERS);
            }
            finally
            {
                PackageQueue.Instance.InputStopped = true;
                TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent reading");
            }           
        }

        static void BobBoxManagerController()
        {
            List<Action> managers = new List<Action>();
            for (int i = 0; i < MAX_BOBBOXMANAGERS; i++)
            {
                int copy = i;
                managers.Add(new Action(() => StartBobBoxManager(copy)));
            }

            TimeStatisticsMapper.Instance.StartTimeMeasure("Time spent saving");
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MAX_BOBBOXMANAGERS }, managers.ToArray());
            TimeStatisticsMapper.Instance.StopTimeMeasure("Time spent saving");

            Console.WriteLine("Done consuming.");

            TimeStatisticsMapper.Instance.StopTimeMeasure("Total time");

            TimeStatisticsMapper.Instance.StartTimeMeasure("Total Crapper time");
            Crapper.Crapper.CleanUp(StartTime);
            TimeStatisticsMapper.Instance.StopTimeMeasure("Total Crapper time");

            Logger.Instance.WriteStatistics();       
        }

        static void StartBobBoxManager(int id)
        {
            Console.WriteLine("Started a BobBoxManager.");
            BobboxManager bbm = new BobboxManager();
            bbm.StartValidatingAndSaving(id);
        }

        static void Initialize()
        {
            // Initialize all the values for the static variables in the Statics class. These
            // variables are used throughout the whole program.

            GlobalVariables.Initialize();
            MAX_BOBS = Properties.PropertyList["max_bob_threads"].GetValue<int>();
            MAX_BOBBOXMANAGERS = Properties.PropertyList["max_bobbox_threads"].GetValue<int>();
            MAX_READERS = Properties.PropertyList["max_reader_threads"].GetValue<int>();


            string dbName = Properties.PropertyList["db_name"].GetValue<string>();
            string dbPassword = Properties.PropertyList["db_password"].GetValue<string>();
            string dbSource = Properties.PropertyList["db_source"].GetValue<string>();
            string dbUserId = Properties.PropertyList["db_userid"].GetValue<string>();
            int dbPort = Properties.PropertyList["db_port"].GetValue<int>();
            int maxPoolSize = Properties.PropertyList["db_max_pool_size"].GetValue<int>();
            BetsyDbContextReader reader =  new BetsyDbContextReader(dbName, dbPassword, dbSource, dbUserId, dbPort, maxPoolSize);
            Lookup.WebshopLookup = reader.GetAllWebshops();
            Lookup.CategoryLookup = reader.GetAllCategories();
            reader.Dispose();

            Logger.LogPath = Properties.PropertyList["log_path"].GetValue<string>();
            TimeStatisticsMapper.Instance.StartTimeMeasure("Total time");
        }
    }
}
