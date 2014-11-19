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
using BorderSource.Queue;

namespace BobAndFriends.BobAndFriends
{
    public class ProductFeedReader
    {
        /// <summary>
        /// Standard path where productfeeds are stored.
        /// </summary>
        private string _productFeedPath;

        private ProductFilter _filter;

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
            _filter = new ProductFilter();
            _filter.LogProperties = false;
        }

        /// <summary>
        /// Start method to start reading and processing productfeeds.
        /// </summary>
        public void Start()
        {
                  
            //Get all the directories in the productfeed folder.
            string[] dirs = Directory.GetDirectories(_productFeedPath);

            Console.WriteLine("Started reading files...");

            //To read all the data from the affiliates, we simply loop through all the classes which are a
            //subclass of AffiliateBase. These classes have references to their data file storage and contain
            //methods which contain the structure of the XML files.

            List<Product> WrongProducts = new List<Product>();

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
                        //Push all the products to the Queue so BOB can process them.
                        foreach(Product p in products)
                        {
                            GeneralStatisticsMapper.Instance.Increment("Products read");                          
                            if (!_filter.CheckProperties(p))
                            {
                                //GeneralStatisticsMapper.Instance.Increment("Products with wrong properties");
                                //WrongProducts.Add(p);
                                continue;
                            }
                            ProductQueue.Instance.Enqueue(p);
                            //GeneralStatisticsMapper.Instance.Increment("Enqueued products");                           
                        }

                        //foreach (Product wp in WrongProducts) products.Remove(wp);
                        //while (PackageQueue.Instance.Queue.Count > Statics.maxQueueSize) Thread.Sleep(300000);
                        //if (products.Count > 0) PackageQueue.Instance.Enqueue(Package.CreateNew(products));
                        //WrongProducts.Clear();
                        //products.Clear();
                    }
                }
            }            
            Console.WriteLine("Done reading productfeeds.");

            //Flag the boolean to be true when finished.
            //PackageQueue.Instance.InputStopped = true;
            ProductQueue.Instance.InputStopped = true;
        }
    }
}
