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
using BorderSource.AffiliateReader;
using BorderSource.AffiliateFile;
using BorderSource.ProductAssociation;
using BorderSource.Statistics;
using BobAndFriends.Global;
using BorderSource.Property;
using BorderSource.Loggers;


namespace BobAndFriends.BobAndFriends
{
    public class ProductFeedReader
    {
        /// <summary>
        /// Standard path where productfeeds are stored.
        /// </summary>
        private string _productFeedPath;

        private ProductFilter _filter;

        private int _maxQueueSize;

        /// <summary>
        /// Constructor for creating FeedReaderController object.
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
            _productFeedPath = Properties.PropertyList["feed_path"].GetValue<string>();
            _filter = new ProductFilter();
            _filter.LogProperties = false;
            Dictionary<string, int> maximums = new Dictionary<string, int>();
            foreach(var pair in Properties.PropertyList.Where(p => p.Value is Property<int>))
            {
                maximums.Add(pair.Key, pair.Value.GetValue<int>());
            }
            _filter.Maximums = maximums;
            _maxQueueSize = Properties.PropertyList["max_packagequeue_size"].GetValue<int>();

            HashSet<string> taxExclusiveWebshops = new HashSet<string>();
            taxExclusiveWebshops.Add("www.hardware.nl");
            taxExclusiveWebshops.Add("www.salland.eu");
            taxExclusiveWebshops.Add("www.maxict.nl");
            _filter.TaxExclusiveWebshops = taxExclusiveWebshops;
        }

        /// <summary>
        /// Start method to start reading and processing productfeeds.
        /// </summary>
        public void Start(int max_readers)
        {              
            //Get all the directories in the productfeed folder.
            string[] dirs = Directory.GetDirectories(_productFeedPath);

            Console.WriteLine("Started retrieving files.");

            List<AffiliateFileBase> files = new List<AffiliateFileBase>();

            foreach(string dir in dirs)
            {
                string[] dirfiles = Directory.GetFiles(dir);
                foreach(string file in dirfiles)
                {
                    string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                    if (!Lookup.WebshopLookup.Contains(fileUrl))
                    {
                        Logger.Instance.WriteLine("Could not find webshop " + fileUrl + " from " + dir);
                        continue;
                    }
                    AffiliateFileBase afFile = new AffiliateXmlFile();
                    afFile.Name = dir.Split(Path.DirectorySeparatorChar).Last();
                    afFile.FileLocation = file;
                    files.Add(afFile);
                }
            }

            var sortedBySize = (from file in files orderby new FileInfo(file.FileLocation).Length descending select file).ToArray<AffiliateFileBase>();

            Console.WriteLine("Creating readers for " + files.Count + " webshops.");
            List<Action> actions = new List<Action>();

            for(int i = 0; i < files.Count; i++)
            {                
                int copy = i;
                actions.Add(new Action(() => ReadFile(sortedBySize[copy])));
            }

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = max_readers }, actions.ToArray());
            Console.WriteLine("Done reading productfeeds.");

            //Flag the boolean to be true when finished.
            PackageQueue.Instance.InputStopped = true;
        }

        public void ReadFile(AffiliateFileBase file)
        {
            List<Product> WrongProducts = new List<Product>();
            AffiliateReaderBase reader = file.GetReader();            

            if(reader == null)
            {
                Console.WriteLine("Didn't find suitable reader for " + Path.GetFileNameWithoutExtension(file.FileLocation).Split(null)[0].Replace('$', '/') + " which should be in " + file.Name);
                return;
            }

            reader.PackageSize = Properties.PropertyList["package_size"].GetValue<int>();
            Console.WriteLine("Started reading " + Path.GetFileNameWithoutExtension(file.FileLocation).Split(null)[0].Replace('$', '/'));
            foreach(List<Product> products in reader.ReadFromFile(file.FileLocation))
            {
                foreach (Product p in products)
                {
                    GeneralStatisticsMapper.Instance.Increment("Products read");
                    if (!_filter.CheckProperties(p))
                    {
                        WrongProducts.Add(p);
                        continue;
                    }
                    if (!GlobalVariables.UniqueIds.Contains(p.AffiliateProdID)) GlobalVariables.UniqueIds.Add(p.AffiliateProdID);
                    else
                    {
                        WrongProducts.Add(p);
                        continue;
                    }
                }
                GeneralStatisticsMapper.Instance.Increment("Wrong products", WrongProducts.Count);

                foreach (Product wp in WrongProducts)
                {
                    products.Remove(wp);
                }

                GeneralStatisticsMapper.Instance.Increment("Correct products", products.Count);
                while (_maxQueueSize == 0 ? false : PackageQueue.Instance.Queue.Count >  _maxQueueSize) Thread.Sleep(1000);
                if (products.Count > 0)
                {
                    Package package = new Package();
                    package.products = new List<Product>(products);
                    package.Webshop = package.products.First().Webshop;
                    PackageQueue.Instance.Enqueue(package);
                }
                WrongProducts.Clear();
                products.Clear();
            }
        }

        public void ReadAffiliate(AffiliateReaderBase af)
        {
            List<Product> WrongProducts = new List<Product>();

            //Invoke ReadFromDir() and read all products
            foreach (List<Product> products in af.ReadFromDir(_productFeedPath + @"\\" + af.Name))
            {
                //Push all the products to the Queue so BOB can process them.
                foreach (Product p in products)
                {
                    GeneralStatisticsMapper.Instance.Increment("Products read");
                    if (!_filter.CheckProperties(p))
                    {
                        WrongProducts.Add(p);
                        continue;
                    }
                    if (!GlobalVariables.UniqueIds.Contains(p.AffiliateProdID)) GlobalVariables.UniqueIds.Add(p.AffiliateProdID);
                    else
                    {
                        WrongProducts.Add(p);
                        continue;
                    }
                }
                GeneralStatisticsMapper.Instance.Increment("Wrong products", WrongProducts.Count);
                foreach (Product wp in WrongProducts) products.Remove(wp);
                while (PackageQueue.Instance.Queue.Count > Properties.PropertyList["max_queue_size"].GetValue<int>()) Thread.Sleep(300000);
                if (products.Count > 0)
                {
                    Package package = new Package();
                    package.products = new List<Product>(products);
                    package.Webshop = package.products.First().Webshop;
                    PackageQueue.Instance.Enqueue(package);
                }
                WrongProducts.Clear();
                products.Clear();
            }
        }
    }
}
