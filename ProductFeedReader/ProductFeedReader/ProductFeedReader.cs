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
using System.Threading;

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
        /// A standard string which contains the replacement value for bad EAN codes.
        /// </summary>
        private const string _EANREPLACEMENT = "";

        /// <summary>
        /// An integer used for storing the total amount of products.
        /// This is only used for logging/statistics.
        /// </summary>
        private int _amountOfProducts;

        /// <summary>
        /// An XMLReader object for streaming xml data.
        /// </summary>
        private XmlReader _reader;

        /// <summary>
        /// A StreamWrites which writes log text to a .txt file.
        /// </summary>
        private StreamWriter _logger;

        /// <summary>
        /// A Stopwatch for measuring the elapsed time (useful fo optimizing).
        /// </summary>
        private Stopwatch _sw;

        /// <summary>
        /// An integer to keep the amount of ticks, for sleeping.
        /// </summary>
        private int _tickCount;

        /// <summary>
        /// This integer represent the amount of ticks neccessary to sleep for 1 ms.
        /// </summary>
        private const int _SLEEPCOUNT = 50000;

        /// <summary>
        /// Constructor for creating ProductFeedReader object.
        /// </summary>
        public ProductFeedReader()
        {
             _badEan = 0;
             _logger = new StreamWriter(@"C:\\Product Feeds\\log\\log-"+DateTime.Now.ToString("ddMMyy-HH-mm") + ".txt");
             _sw = new Stopwatch();
            _tickCount = 0;           
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
            foreach (string dir in dirs)
            {
                //Process each by their own name.
                switch (dir)
                {

                    case _PRODUCTFEEDPATH + "\\TradeTracker":

                        #region TradeTracker

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

                        foreach (string file in filePaths)
                        {
                            Console.Write("Started reading from: " + file + " ...");
                            try
                            {
                                _reader = XmlReader.Create(file);
                                Product p = null;
                                while (_reader.Read())
                                {
                                    //Increment the tickcount
                                    _tickCount++;

                                    //Sleep everytime sleepcount is reached
                                    if (_tickCount % _SLEEPCOUNT == 0)
                                    {
                                        Thread.Sleep(1);

                                        //Set tickCount to 0 to save memory
                                        _tickCount = 0;
                                    }
                                   
                                    if (_reader.IsStartElement())
                                    {
                                        switch (_reader.Name)
                                        {
                                            case "product":
                                                p = new Product();
                                                break;

                                            case "name":
                                                    if (_reader.Read())
                                                    {
                                                        p.Name = _reader.Value;
                                                    }
                                                    break;

                                            case "price":
                                                    p.Currency = _reader.GetAttribute("currency");
                                                    if (_reader.Read())
                                                    {
                                                        p.Price = _reader.Value;                                                       
                                                    }
                                                    break;

                                            case "URL":
                                                    if (_reader.Read())
                                                    {
                                                        p.Url = _reader.Value;
                                                    }
                                                    break;

                                            case "image":
                                                    if (_reader.Read())
                                                    {
                                                        p.Image = _reader.Value;
                                                    }
                                                    break;

                                            case "description":
                                                    if (_reader.Read())
                                                    {
                                                        p.Description = _reader.Value;
                                                    }
                                                    break;

                                            case "category":
                                                    if (_reader.Read())
                                                    {
                                                        p.Category = _reader.Value;
                                                    }
                                                    break;

                                            case "property":
                                                if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                                switch (_reader.Value)
                                                {
                                                    case "currency":
                                                        if (_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.Currency = _reader.Value;
                                                        }
                                                        break;

                                                    case "EAN":
                                                        if (_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : _EANREPLACEMENT;
                                                        }
                                                        break;

                                                    case "brand":
                                                        if (_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.Brand = _reader.Value;
                                                        }
                                                        break;

                                                    case "deliveryCosts":
                                                        if (_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.DeliveryCost = _reader.Value;
                                                        }
                                                        break;

                                                    case "stock":
                                                        if (_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.Stock = _reader.Value;
                                                        }
                                                        break;

                                                    case "deliveryTime":
                                                        if (_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.DeliveryTime = _reader.Value;
                                                        }
                                                        break;

                                                    case "SKU":
                                                        if(_reader.Read())
                                                        {
                                                            _reader.Read();
                                                            _reader.Read();
                                                            p.SKU = _reader.Value;
                                                        }
                                                        break;
                                                }
                                                _reader.MoveToElement();
                                                break;
                                        }
                                    }

                                    if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        p.Affiliate = "TradeTracker";
                                        p.FileName = file;
                                        p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0];
                                        products.Add(p);
                                    }
                                }
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

                            //Clear the productlist to solve RAM issues
                            //In the next version, writing to the database will be done here.
                            _amountOfProducts += products.Count;
                            products.Clear();
                        }

                        break;

                        #endregion                       

                    case _PRODUCTFEEDPATH + "\\Zanox":

                        #region Zanox

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

                        foreach (string file in filePaths)
                        {
                            Console.Write("Started reading from: " + file + " ...");
                            try
                            {
                                _reader = XmlReader.Create(file);
                                Product p = null;
                                string lastUpdated = null;
                                string currency = null;
                                while (_reader.Read())
                                {
                                    //Increment the tickcount
                                    _tickCount++;

                                    //Sleep everytime sleepcount is reached
                                    if (_tickCount % _SLEEPCOUNT == 0)
                                    {
                                        Thread.Sleep(1);

                                        //Set tickCount to 0 to save memory
                                        _tickCount = 0;
                                    }
                                    if (_reader.IsStartElement())
                                    {
                                        switch (_reader.Name)
                                        {
                                            case "streamCurrency":
                                                if (_reader.Read())
                                                {
                                                    currency = _reader.Value;
                                                }
                                                break;

                                            case "lastUpdated":
                                                if (_reader.Read())
                                                {
                                                    lastUpdated = _reader.Value;
                                                }
                                                break;

                                            case "record":
                                                p = new Product();
                                                break;

                                            case "column":
                                                if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                                switch (_reader.Value)
                                                {
                                                    case "url":
                                                        if (_reader.Read())
                                                        {
                                                            p.Url = _reader.Value;
                                                        }
                                                        break;

                                                    case "title":
                                                        if(_reader.Read())
                                                        {
                                                            p.Name = _reader.Value;
                                                        }
                                                        break;

                                                    case "ean":
                                                        if (_reader.Read())
                                                        {
                                                            p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : _EANREPLACEMENT;
                                                        }
                                                        break;

                                                    case "price":
                                                        if (_reader.Read())
                                                        {
                                                            p.Price = _reader.Value;
                                                        }
                                                        break;

                                                    case "image":
                                                        if (_reader.Read())
                                                        {
                                                            p.Image = _reader.Value;
                                                        }
                                                        break;

                                                    case "category":
                                                        if (_reader.Read())
                                                        {
                                                            p.Category = _reader.Value;
                                                        }
                                                        break;

                                                    case "description":
                                                        if (_reader.Read())
                                                        {
                                                            p.Description = _reader.Value;
                                                        }
                                                        break;

                                                    case "price_shipping":
                                                        if (_reader.Read())
                                                        {
                                                            p.DeliveryCost = _reader.Value;
                                                        }
                                                        break;

                                                    case "stock":
                                                        if (_reader.Read())
                                                        {
                                                            p.Stock = _reader.Value;
                                                        }
                                                        break;

                                                    case "timetoship":
                                                        if (_reader.Read())
                                                        {
                                                            p.DeliveryTime = _reader.Value;
                                                        }
                                                        break;                                                      
                                                }
                                                _reader.MoveToElement();
                                                break;
                                        }
                                    }

                                    if (_reader.Name.Equals("record") && _reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        
                                        p.Currency = currency;
                                        p.LastModified = lastUpdated;
                                        p.Affiliate = "Zanox";
                                        p.FileName = file;
                                        p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0];
                                        products.Add(p);
                                    }
                                }
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

                            //Clear the productlist to solve RAM issues
                            //In the next version, writing to the database will be done here.
                            _amountOfProducts += products.Count;
                            products.Clear();
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
                                _reader = XmlReader.Create(file);
                                Product p = null;
                                while (_reader.Read())
                                {
                                    //Increment the tickcount
                                    _tickCount++;

                                    //Sleep everytime sleepcount is reached
                                    if (_tickCount % _SLEEPCOUNT == 0)
                                    {
                                        Thread.Sleep(1);

                                        //Set tickCount to 0 to save memory
                                        _tickCount = 0;
                                    }
                                    if (_reader.IsStartElement())
                                    {
                                        switch (_reader.Name)
                                        {
                                            case "EAN":
                                                if (_reader.Read())
                                                {
                                                    p.EAN = _reader.Value;
                                                }
                                                break;

                                            case "Title":
                                                if (_reader.Read())
                                                {
                                                    p.Name = _reader.Value;
                                                }
                                                break;

                                            case "Brand":
                                                if (_reader.Read())
                                                {
                                                    p.Brand = _reader.Value;
                                                }
                                                break;

                                            case "Price":
                                                if (_reader.Read())
                                                {
                                                    p.Price = _reader.Value;
                                                }
                                                break;

                                            case "CurrencySymbol":
                                                if (_reader.Read())
                                                {
                                                    p.Currency = _reader.Value;
                                                }
                                                break;

                                            case "ValidTo":
                                                if (_reader.Read())
                                                {
                                                    p.ValidUntil = _reader.Value;
                                                }
                                                break;

                                            case "Deeplinks":
                                                if (_reader.Read())
                                                {
                                                    if (_reader.Read())
                                                    {
                                                        p.Url = _reader.Value;
                                                    }

                                                    //Read again twice to avoid double products because some genius made the xml file have two Product elements...
                                                    _reader.Read();
                                                    _reader.Read();
                                                }
                                                break;

                                            case "Images":
                                                if (_reader.Read())
                                                {
                                                    _reader.ReadToFollowing("URL");
                                                    if (_reader.Read())
                                                    {
                                                        p.Image = _reader.Value;
                                                    }
                                                }
                                                break;

                                            case "ProductCategoryPath":
                                                if (_reader.Read())
                                                {
                                                    p.Category = _reader.Value;
                                                }
                                                break;

                                            case "Description":
                                                if (_reader.Read())
                                                {
                                                    p.Description = _reader.Value;
                                                }
                                                break;

                                            case "LastUpdate":
                                                if (_reader.Read())
                                                {
                                                    p.LastModified = _reader.Value;
                                                }
                                                break;

                                            case "Shipping":
                                                if (_reader.Read())
                                                {
                                                    _reader.ReadToFollowing("Shipping");
                                                    if (_reader.Read())
                                                    {
                                                        p.DeliveryCost = _reader.Value;
                                                    }
                                                }
                                                break;

                                            case "Properties":
                                                if (_reader.Read())
                                                {
                                                    while(!(_reader.Name.Equals("Properties") && _reader.NodeType == XmlNodeType.EndElement))
                                                    {                                                      
                                                        if(_reader.HasAttributes && _reader["Title"].Equals("STOCK"))
                                                        {
                                                            break;
                                                        }

                                                        _reader.Read();
                                                       
                                                    }
                                                    p.Stock = _reader["Text"] ?? "";
                                                }
                                                break;                                                  

                                            case "Product":                                               
                                                p = new Product();
                                                break;
                                        }
                                    }

                                    if (_reader.Name.Equals("Product") && _reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        p.Affiliate = "Affilinet";
                                        p.FileName = file;
                                        p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0];
                                        products.Add(p);
                                    }
                                }
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

                            //Clear the productlist to solve RAM issues
                            //In the next version, writing to the database will be done here.
                            _amountOfProducts += products.Count;
                            products.Clear();
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
                                _reader = XmlReader.Create(file);
                                Product p = null;
                                while (_reader.Read())
                                {
                                    //Increment the tickcount
                                    _tickCount++;

                                    //Sleep everytime sleepcount is reached
                                    if (_tickCount % _SLEEPCOUNT == 0)
                                    {
                                        Thread.Sleep(1);

                                        //Set tickCount to 0 to save memory
                                        _tickCount = 0;
                                    }
                                    if(_reader.IsStartElement())
                                    {
                                        switch(_reader.Name)
                                        {
                                            case "ean":
                                                if (_reader.Read())
                                                {
                                                    p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : _EANREPLACEMENT;
                                                }
                                                break;

                                            case "productname":
                                                if (_reader.Read())
                                                {
                                                    p.Name = _reader.Value;
                                                }
                                                break;

                                            case "brandname":
                                                if (_reader.Read())
                                                {
                                                    p.Brand = _reader.Value;
                                                }
                                                break;

                                            case "currentprice":
                                                if (_reader.Read())
                                                {
                                                    p.Price = _reader.Value;
                                                }
                                                break;

                                            case "currency":
                                                if (_reader.Read())
                                                {
                                                    p.Currency = _reader.Value;
                                                }
                                                break;

                                            case "validuntil":
                                                if (_reader.Read())
                                                {
                                                    p.ValidUntil = _reader.Value;
                                                }
                                                break;

                                            case "deeplinkurl":
                                                if (_reader.Read())
                                                {
                                                    p.Url = _reader.Value;
                                                }
                                                break;

                                            case "imagesmallurl":
                                                if (_reader.Read())
                                                {
                                                    p.Image = _reader.Value;
                                                }
                                                break;

                                            case "productcategory":
                                                if (_reader.Read())
                                                {
                                                    p.Category = _reader.Value;
                                                }
                                                break;

                                            case "productdescriptionslong":
                                                if (_reader.Read())
                                                {
                                                    p.Description = _reader.Value;
                                                }
                                                break;

                                            case "lastupdate":
                                                if (_reader.Read())
                                                {
                                                    p.LastModified = _reader.Value;
                                                }
                                                break;

                                            case "shipping":
                                                if (_reader.Read())
                                                {
                                                    p.DeliveryCost = _reader.Value;
                                                }
                                                break;

                                            case "availability":
                                                if (_reader.Read())
                                                {
                                                    p.Stock = _reader.Value;
                                                }
                                                break;

                                            case "product":
                                                p = new Product();
                                                break;
                                        }
                                    }

                                    if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        p.Affiliate = "Belboon";
                                        p.FileName = file;
                                        p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0];
                                        products.Add(p);
                                    }
                                }
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

                            //Clear the productlist to solve RAM issues
                            //In the next version, writing to the database will be done here.
                            _amountOfProducts += products.Count;
                            products.Clear();

                        }
                        break;

                        #endregion    
                  
                    case _PRODUCTFEEDPATH + "\\Webgains":

                        #region Webgains

                        filePaths = ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

                        foreach (string file in filePaths)
                        {
                            Console.Write("Started reading from: " + file + " ...");
                            try
                            {
                                _reader = XmlReader.Create(file);
                                Product p = null;
                                while (_reader.Read())
                                {
                                    //Increment the tickcount
                                    _tickCount++;

                                    //Sleep everytime sleepcount is reached
                                    if (_tickCount % _SLEEPCOUNT == 0)
                                    {
                                        Thread.Sleep(1);

                                        //Set tickCount to 0 to save memory
                                        _tickCount = 0;
                                    }
                                    if (_reader.IsStartElement())
                                    {
                                        switch (_reader.Name)
                                        {
                                            case "european_article_number":
                                                if (_reader.Read())
                                                {
                                                    p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : _EANREPLACEMENT;
                                                }
                                                break;

                                            case "product_name":
                                                if (_reader.Read())
                                                {
                                                    p.Name = _reader.Value;
                                                }
                                                break;

                                            case "brand":
                                                if (_reader.Read())
                                                {
                                                    p.Brand = _reader.Value;
                                                }
                                                break;

                                            case "price":
                                                if (_reader.Read())
                                                {
                                                    p.Price = _reader.Value;
                                                }
                                                break;

                                            case "currency":
                                                if (_reader.Read())
                                                {
                                                    p.Currency = _reader.Value;
                                                }
                                                break;

                                            case "validuntil":
                                                if (_reader.Read())
                                                {
                                                    p.ValidUntil = _reader.Value;
                                                }
                                                break;

                                            case "deeplink":
                                                if (_reader.Read())
                                                {
                                                    p.Url = _reader.Value;
                                                }
                                                break;

                                            case "image_url":
                                                if (_reader.Read())
                                                {
                                                    p.Image = _reader.Value;
                                                }
                                                break;

                                            case "category":
                                                if (_reader.Read())
                                                {
                                                    p.Category = _reader.Value;
                                                }
                                                break;

                                            case "description":
                                                if (_reader.Read())
                                                {
                                                    p.Description = _reader.Value;
                                                }
                                                break;

                                            case "last_updated":
                                                if (_reader.Read())
                                                {
                                                    p.LastModified = _reader.Value;
                                                }
                                                break;

                                            case "delivery_cost":
                                                if (_reader.Read())
                                                {
                                                    p.DeliveryCost = _reader.Value;
                                                }
                                                break;

                                            case "delivery_period":
                                                if (_reader.Read())
                                                {
                                                    p.DeliveryTime = _reader.Value;
                                                }
                                                break;

                                            case "product":
                                                p = new Product();
                                                break;
                                        }
                                    }

                                    if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        p.Affiliate = "Webgains";
                                        p.FileName = file;
                                        p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0];
                                        products.Add(p);
                                    }
                                }
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

                            //Clear the productlist to solve RAM issues
                            //In the next version, writing to the database will be done here.
                            _amountOfProducts += products.Count;
                            products.Clear();

                        }
                        break;

                        #endregion  
                    default: break;
                }
            }
       
            _sw.Stop();

            _logger.WriteLine("Last scan: " + DateTime.Now.ToString("HH:mm:ss") + ".");
            _logger.WriteLine("Processing time: " + _sw.Elapsed);
            _logger.WriteLine(_amountOfProducts + " products processed.");
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

        #endregion
    }
}
