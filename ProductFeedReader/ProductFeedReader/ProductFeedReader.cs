using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Reflection;

namespace ProductFeedReader
{
    public class ProductFeedReader
    {
        /// <summary>
        /// Standard path where productfeeds are stored.
        /// </summary>
        private string _productFeedPath;

        /// <summary>
        /// PAth where logfiles are to be written.
        /// </summary>
        private string _logPath;

        /// <summary>
        /// The Database object containing methods to rite products to the database.
        /// </summary>
        private Database _db;

        /// <summary>
        /// The settings of the ProductFeedReader.
        /// </summary>
        private INIFile _ini;

        private Dictionary<string, string> _settings;

        public static bool isDone;

        /// <summary>
        /// Constructor for creating ProductFeedReader object.
        /// </summary>
        public ProductFeedReader()
        {
            Initialize();
        }

        private void Initialize()
        {
            //Get all the settings from the INI-file
            _ini = new INIFile("C:\\Product Feeds\\settings\\pfr.ini");
            _settings = _ini.GetAllValues();

            //Read settings
            _productFeedPath = _settings["productfeedpath"];
            _logPath = _settings["logpath"];

            Util.Logger = new Logger(_logPath + "\\log-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            Util.SleepCount = Int32.Parse(_settings["sleepcount"]);

            _db = new Database();

            isDone = false;          
        }

        /// <summary>
        /// Start method to start reading and processing productfeeds.
        /// </summary>
        public void Start()
        {
            Util.Logger.StartStopwatch();

            Console.WriteLine("Opening connection with database...");

            try
            {
                //Open the database connection. The program should stop if this fails.
                _db.Connect(_settings["dbsource"], _settings["dbname"], _settings["dbuid"], _settings["dbpw"]);
            }
            catch(Exception e)
            {
                Console.WriteLine("Connection failed.");
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("From: " + e.Source);
                Console.WriteLine("Press any key to exit.");
                Console.Read();
                Environment.Exit(0);
            }

            Console.WriteLine("Connection opened.");         
            
            //Get all the directories in the productfeed folder.
            string[] dirs = Directory.GetDirectories(_productFeedPath);

            Console.WriteLine("Started reading files...");

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach(Type type in types)
            {
                if(type.IsSubclassOf(typeof(AffiliateBase)))
                {
                    AffiliateBase af = (AffiliateBase)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    foreach (List<Product> products in af.ReadFromDir(_productFeedPath + @"\\" + af.Name))
                    {
                        Util.Logger.AddStats(products);
                        foreach(Product p in products)
                        {
                            ProductQueue.Enqueue(p);
                        }
                    }
                }
            }            
            Console.WriteLine("Closing connection with database...");

            //Close the database.
            _db.Close();

            Console.WriteLine("Connection closed.");

            Console.WriteLine("Writing data to logfile...");
            Util.Logger.Close();
            Console.WriteLine("Done.");
            isDone = true;
        }
    }
}
