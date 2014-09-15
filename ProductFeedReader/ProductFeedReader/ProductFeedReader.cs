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
            //Read settings
            _productFeedPath = Statics.settings["productfeedpath"];
            isDone = false;          
        }

        /// <summary>
        /// Start method to start reading and processing productfeeds.
        /// </summary>
        public void Start()
        {
            Statics.Logger.StartStopwatch();
                  
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
                        Statics.Logger.AddStats(products);
                        foreach(Product p in products)
                        {
                            ProductQueue.Enqueue(p);
                        }
                    }
                }
            }            

            Console.WriteLine("Connection closed.");

            Console.WriteLine("Writing data to logfile...");
            Statics.Logger.Close();
            Console.WriteLine("Done.");
            isDone = true;
        }
    }
}
