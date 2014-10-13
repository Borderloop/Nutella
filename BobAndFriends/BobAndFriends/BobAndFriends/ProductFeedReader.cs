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
using BorderSource.Common;

namespace BobAndFriends
{
    public class ProductFeedReader
    {
        /// <summary>
        /// Standard path where productfeeds are stored.
        /// </summary>
        private string _productFeedPath;

        /// <summary>
        /// This flag is used to singal other threads that this thread is done.
        /// </summary>
        public static bool isDone;

        /// <summary>
        /// Constructor for creating ProductFeedReader object.
        /// </summary>
        public ProductFeedReader()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize() will set up all variables that are needed in this class
        /// </summary>
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
            //Start the stopwatch for time measuring
            Statics.Logger.StartStopwatch();
                  
            //Get all the directories in the productfeed folder.
            string[] dirs = Directory.GetDirectories(_productFeedPath);

            Console.WriteLine("Started reading files...");

            //To read all the data from the affiliates, we simply loop through all the classes which are a
            //subclass of AffiliateBase. These classes have references to their data file storage and contain
            //methods which contain the structure of the XML files.

            //Get all types in the assembly
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            //Loop through each type
            foreach(Type type in types)
            {
                //If a type is a subclass of AffiliateBase, select it.
                if(type.IsSubclassOf(typeof(AffiliateBase)))
                {
                    //Create an instance of the subclass by invoking the constructor.
                    AffiliateBase af = (AffiliateBase)type.GetConstructor(Type.EmptyTypes).Invoke(null);

                    //Invoke ReadFromDir() and read all products
                    foreach (List<BorderSource.Common.Product> products in af.ReadFromDir(_productFeedPath + @"\\" + af.Name))
                    {
                        //Save some data to the logger for statistics.
                        //Statics.Logger.AddStats(products);

                        //Push all the products to the queue so BOB can process them.
                        foreach(Product p in products)
                        {
                            while(ProductQueue.queue.Count > Statics.maxQueueSize)
                            {
                                Thread.Sleep(10000);
                            }
                            if (!p.CleanupFields())
                            {
                                Statics.Logger.WriteLine("Product has invalid properties.");
                                Statics.Logger.WriteLine(p.ToString());
                                continue;
                            }

                            ProductQueue.Enqueue(p);
                        }
                    }
                }
            }            
            Console.WriteLine("Done reading productfeeds.");

            //Flag the boolean to be true when finished.
            isDone = true;
        }
    }
}
