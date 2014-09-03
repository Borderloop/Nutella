using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ProductFeedReader
{
    public class ProductFeedReader
    {
        /// <summary>
        /// Standard path where productfeeds are stored.
        /// </summary>
        private const string _PRODUCTFEEDPATH = @"C:\\Product Feeds";

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
        /// A Stopwatch for measuring the elapsed time (useful fo optimizing).
        /// </summary>
        private Stopwatch _sw;

        /// <summary>
        /// Constructor for creating ProductFeedReader object.
        /// </summary>
        public ProductFeedReader()
        {
             _badEan = 0;
             _logger = new StreamWriter(@"C:\\Product Feeds\\log.txt");
             _sw = new Stopwatch();
        }

        /// <summary>
        /// Start method to start reading and processing productfeeds.
        /// </summary>
        public void Start()
        {
            _sw.Start();
            //Get all the directories in the productfeed folder.
            string[] dirs = Directory.GetDirectories(_PRODUCTFEEDPATH);

            List<Product> products = new List<Product>();

            string[] filePaths;

            //Loop through each
            foreach(string dir in dirs)
            {
                //Process each by their own name.
                switch(dir)
                {
                        
                        case _PRODUCTFEEDPATH+"\\TradeTracker":
                        
                        #region TradeTracker

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));
                       
                            foreach (string file in filePaths)
                            { 
                                try
                                {
                                    Console.Write("Started reading from: " + file + " ...");
                                    products.AddRange(
                                     (
                                         from e in XDocument.Load(file).Root.Elements("product")
                                         select new Product
                                         {
                                             Name = (string)e.Element("name"),
                                             Url = (string)e.Element("URL"),
                                             Image = (string)e.Element("images").Element("image"),
                                             Description = (string)e.Element("description"),
                                             Category = (string)e.Element("categories").Element("category"),
                                             Price = (string)e.Element("price"),
                                             Currency = (string)e.Element("price").Attribute("currency").Value,
                                             DeliveryCost = SearchTradeTrackerProperty(e, "deliveryCost"),
                                             DeliveryTime = SearchTradeTrackerProperty(e, "deliveryTime"),
                                             EAN = SearchTradeTrackerProperty(e, "EAN"),
                                             Stock = SearchTradeTrackerProperty(e, "stock"),
                                             Brand = SearchTradeTrackerProperty(e, "brand"),
                                             LastModified = "",
                                             ValidUntil = "",
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
                                Console.WriteLine(" Done");
                            }
                        
                        break;

                        #endregion
                        

                        case _PRODUCTFEEDPATH+"\\Zanox":

                        #region Zanox

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));
                       
                            foreach (string file in filePaths)
                            {
                                Console.Write("Started reading from: " + file + " ...");
                                try
                                {
                                    XElement root = XDocument.Load(file).Root;
                                    string currency = root.Element("dataHeader").Element("streamCurrency").Value;
                                    string lastModified = root.Element("dataHeader").Element("lastUpdated").Value;
                                    products.AddRange(
                                     (
                                         from e in root.Elements("data").Elements("record")
                                         select new Product
                                         {
                                             Name = SearchZanoxColumn(e, "title"),
                                             Url = SearchZanoxColumn(e, "url"),
                                             Image = SearchZanoxColumn(e, "image"),
                                             Description = SearchZanoxColumn(e, "description"),
                                             Category = SearchZanoxColumn(e, "category"),
                                             Price = SearchZanoxColumn(e, "price"),
                                             Currency = currency,                                           
                                             DeliveryCost = SearchZanoxColumn(e, "price_shipping"),
                                             DeliveryTime = SearchZanoxColumn(e, "timetoship"),
                                             EAN = SearchZanoxColumn(e, "ean"),
                                             Stock = SearchZanoxColumn(e, "stock"),
                                             Brand = "",
                                             LastModified = lastModified,
                                             ValidUntil = SearchZanoxColumn(e, "time"),
                                             Affiliate = "Zanox",
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
                                Console.WriteLine(" Done");
                            }      
                        break;

                        #endregion
                        
                        case _PRODUCTFEEDPATH + "\\Affilinet":

                        #region Affilinet

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

                        foreach (string file in filePaths)
                        {
                            Console.Write("Started reading from: " + file + " ...");
                            try
                            {
                                XElement root = XDocument.Load(file).Root;
                                products.AddRange(
                                 (
                                     from e in root.Elements("Product")
                                     select new Product
                                     {
                                         Name = (string)e.Element("Details").Element("Title").Value,
                                         Url = (string)e.Element("Deeplinks").Element("Product").Value,
                                         Image = (string)e.Element("Images").Elements("Img").First().Value,
                                         Description = (string)e.Element("Details").Element("Description").Value,
                                         Category = (string)e.Element("CategoryPath").Element("ProductCategoryPath").Value,
                                         Price = (string)e.Element("Price").Element("Price").Value,
                                         Currency = (string)e.Element("Price").Element("CurrencySymbol").Value,                                      
                                         DeliveryCost = (string)e.Element("Shipping").Element("Shipping").Value,
                                         DeliveryTime = "",
                                         EAN = Regex.IsMatch((string)e.Element("Details").Element("EAN").Value, @"^[0-9]{10,13}$") ? (string)e.Element("Details").Element("EAN").Value : "",
                                         Stock = (string)e.Element("Properties").Elements("Property").FirstOrDefault(x => x.HasAttributes && x.Attribute("Title").Value == "STOCK") != null ? 
                                                 (string)e.Element("Properties").Elements("Property").FirstOrDefault(x => x.HasAttributes && x.Attribute("Title").Value == "STOCK").Attribute("Text").Value : "",
                                         Brand = (string)e.Element("Details").Element("Brand").Value,
                                         LastModified = (string)e.Element("Date").Element("LastUpdate").Value,
                                         ValidUntil = (string)e.Element("Date").Element("ValidTo").Value,
                                         Affiliate = "Affilinet",
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
                            Console.WriteLine(" Done");
                        }
                        break;

                        #endregion
                        
                        case _PRODUCTFEEDPATH + "\\Belboon":

                        #region Belboon

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

                        foreach (string file in filePaths)
                        {
                            Console.Write("Started reading from: " + file + " ...");
                            try
                            {
                                XElement root = XDocument.Load(file).Root;
                                products.AddRange(
                                 (
                                     from e in root.Elements("product")
                                     select new Product
                                     {
                                         Name = (string)e.Element("productname").Value,
                                         Url = (string)e.Element("deeplinkurl").Value,
                                         Image = (string)e.Element("imagesmallurl").Value,
                                         Description = (string)e.Element("productdescriptionslong").Value,
                                         Category = (string)e.Element("productcategory").Value,
                                         Price = (string)e.Element("currentprice").Value,
                                         Currency = (string)e.Element("currency").Value,                                       
                                         DeliveryCost = (string)e.Element("shipping").Value,
                                         DeliveryTime = "",
                                         EAN = Regex.IsMatch((string)e.Element("ean").Value, @"^[0-9]{10,13}$") ? (string)e.Element("ean").Value : "",
                                         Stock = (string)e.Element("availability").Value,
                                         Brand = (string)e.Element("brandname").Value,
                                         LastModified = (string)e.Element("lastupdate").Value,
                                         ValidUntil = (string)e.Element("validuntil").Value,
                                         Affiliate = "Belboon",
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
                            Console.WriteLine(" Done");
                        }
                        break;

                        #endregion

                        default: break;
                }
            }

            /*
            #region EAN Filter
            ///First version of algortihm for creating null-products
            ///It will compare all EAN numbers to all other EAN numbers, read their product names, split those in words and see if these words
            ///match other products too.

            //First, sort the list
            List<Product> sortedOnEan = products.OrderBy(p => p.EAN).ToList();

            //Create a new list without bad EANs
            List<Product> EANList = new List<Product>();
            foreach(Product p in sortedOnEan)
            {
                if(!p.EAN.Trim().Equals(""))
                {
                    EANList.Add(p);
                }
            }

            //Create a new list with only 1 EAN code and a union of the words in the names of the products
            List<Product> combinedList = new List<Product>();
            int count = 0;
            foreach(Product p in EANList)
            {
                if(p.Equals(EANList.First()))
                {
                    combinedList.Add(p);
                    count++;
                }

                if (p.EAN.Equals(combinedList[count-1].EAN))
                {
                    string similars = GetSimilarWords(p.Name, combinedList[count - 1].Name);
                    if (similars.Equals("") || similars.Split(null).Length < 4)
                    {
                        combinedList[count - 1].Name = p.Name.Length < combinedList[count - 1].Name.Length ? p.Name : combinedList[count - 1].Name;
                    }
                    else
                    {
                        combinedList[count - 1].Name = similars;
                    }
                }
                else
                {
                    combinedList.Add(p);
                    count++;
                }
            }

            #endregion         
            */
            _sw.Stop();
            Console.WriteLine("Processing time: " + _sw.Elapsed);

            _logger.WriteLine("Last scan: " + DateTime.Now.ToString("HH:mm:ss") + ".");
            _logger.WriteLine("Processing time: " + _sw.Elapsed);
            _logger.WriteLine(products.Count + " products processed.");
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

        public string GetSimilarWords(string x, string y)
        {
            string sim = "";          

            //Split both strings over whitespaces
            string[] xSplit = x.Split(null);
            string[] ySplit = y.Split(null);

            //Convert all trings to lowercase
            string[] xLower = xSplit.Select(s => s.ToLower()).ToArray();
            string[] yLower = ySplit.Select(s => s.ToLower()).ToArray();

            //Create new string array to store similarities
            string[] simArr = new string[Math.Max(xSplit.Length, ySplit.Length)];
            int wordCount = 0;
            for (int i = 0; i < xLower.Length; i++)
            {
                for (int j = 0; j < yLower.Length; j++)
                {
                    if (xLower[i].Equals(yLower[j]))
                    {
                        //Use xSplit so remain capital letters
                        simArr[wordCount] = xSplit[i]; //Or ySplit[j], they are the same.
                        wordCount++;
                        break;
                    }
                }
            }

            //Concatinate all the strings in simArr with whiteSpaces.
            foreach(string s in simArr)
            {
                sim += s + " ";
            }

            //Trim for sure
            return sim.Trim(); ;
        }

       
        public string SearchTradeTrackerProperty(XElement e, string propname)
        {
            XElement element = e.Element("properties").Elements("property").FirstOrDefault(x => x.HasAttributes && x.Attribute("name").Value == propname);
            if(element != null && propname.Equals("EAN"))
            {
                if(!Regex.IsMatch(element.Value, @"^[0-9]{10,13}$"))
                {
                    return "";
                }
            }
            return element != null ? element.Value : "";
        }

        public string SearchZanoxColumn(XElement e, string colname)
        {
            XElement element = e.Elements("column").FirstOrDefault(x => x.HasAttributes && x.Attribute("name").Value == colname);
            if (element == null && colname.Equals("ean"))
            {
                _badEan++;
            }
            if (element != null && colname.Equals("ean"))
            {
                if (!Regex.IsMatch(element.Value, @"^[0-9]{10,13}$"))
                {
                    return "";
                }
            }
            return element != null ? element.Value : "";
        }


        #endregion
    }
}
