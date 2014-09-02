using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ProductFeedReader
{
    public class ProductFeedReader
    {
        /// <summary>
        /// Standard path where productfeeds are stored.
        /// </summary>
        private const string _PRODUCTFEEDPATH = "C:\\Productfeeds";

        /// <summary>
        /// A simple integer holding the count of products which do not have an EAN number. 
        /// This is only used for logging/statistics.
        /// </summary>
        private int _badEan;

        /// <summary>
        /// A StreamWrites which writes log text to a .txt file.
        /// </summary>
        private StreamWriter _logger;

        /// <summary>
        /// Constructor for creating ProductFeedReader object.
        /// </summary>
        public ProductFeedReader()
        {
             _badEan = 0;
             _logger = new StreamWriter("C:\\Productfeeds\\log.txt");  
        }

        /// <summary>
        /// Start method to start reading and processing productfeeds.
        /// </summary>
        public void Start()
        {
            //Get all the directories in the productfeed folder.
            string[] dirs = Directory.GetDirectories(_PRODUCTFEEDPATH);

            List<Product> products = new List<Product>();

            //Loop through each
            foreach(string dir in dirs)
            {
                //Process each by their own name.
                switch(dir)
                {
                    case _PRODUCTFEEDPATH+"\\TradeTracker":
                        #region TradeTracker

                        string[] filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));
                       
                            foreach (string file in filePaths)
                            { 
                                try
                                {
                                    products.AddRange(
                                     (
                                         from e in XDocument.Load(file).Root.Elements("product")
                                         select new Product
                                         {
                                             ID = (string)e.Attribute("ID"),
                                             Name = (string)e.Element("name"),
                                             Url = (string)e.Element("URL"),
                                             Image = (string)e.Element("images").Element("image"),
                                             Description = (string)e.Element("description"),
                                             Category = (string)e.Element("categories").Element("category"),
                                             SKU = SearchTradeTrackerProperty(e, "SKU"),
                                             DeliveryCost = SearchTradeTrackerProperty(e, "deliveryCost"),
                                             DeliveryTime = SearchTradeTrackerProperty(e, "deliveryTime"),
                                             EAN = SearchTradeTrackerProperty(e, "EAN"),
                                             Stock = SearchTradeTrackerProperty(e, "stock"),
                                             Brand = SearchTradeTrackerProperty(e, "brand"),
                                             Color = SearchTradeTrackerProperty(e, "color"),
                                             Affiliate = "TradeTracker",
                                             FileName = file
                                         }).ToList());
                                }
                                catch (XmlException xmle)
                                {
                                    _logger.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                                }
                                catch (Exception e)
                                {
                                    _logger.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                                }
                            }                     
                        break;

                        #endregion

                    default: break;
                }
            }

                                      
            _logger.WriteLine("\nBAD EAN AMOUNT: " + _badEan + ", AMOUNT OF PRODUCTS: " + products.Count);
            _logger.WriteLine("BAD EAN PERC: " + (_badEan * 100) / products.Count + "%");
            _logger.Close();
        }

        #region Utility Methods

        public static string[] ConcatArrays(string[] x, string[] y)
        {
            if (x == null) throw new ArgumentNullException("First argument is null.");
            if (y == null) throw new ArgumentNullException("Second argument is null.");
            int oldLen = x.Length;
            Array.Resize<string>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }

        public string SearchTradeTrackerProperty(XElement e, string propname)
        {
            XElement element = e.Element("properties").Elements("property").FirstOrDefault(x => x.HasAttributes && x.Attribute("name").Value == propname);
            if (element == null && propname.Equals("EAN"))
            {
                _badEan++; 
            }
            if(element != null && propname.Equals("EAN"))
            {
                if(!Regex.IsMatch(element.Value, @"^[0-9]{10,13}$"))
                {
                    _badEan++;
                }
            }
            return element != null ? element.Value : "";
        }

        #endregion
    }
}
