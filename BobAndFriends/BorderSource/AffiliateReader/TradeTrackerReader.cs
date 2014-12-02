using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using BorderSource.Common;
using BorderSource.ProductAssociation;
using BorderSource.Loggers;

namespace BorderSource.AffiliateReader
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by ZanoxReader.
    /// This reading cannot be automated with the XmlValueReader because the xml
    /// is delivered too complicated.
    /// </summary>
    public class TradeTrackerReader : AffiliateReaderBase
    {
        public override string Name { get { return "Tradetracker"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {


            using (XmlReader _reader = XmlReader.Create(file, new XmlReaderSettings { CloseInput = true }))
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                Product p = null;
                bool isDone = false;
                bool nextLoop = false;
                while (!isDone)
                {
                    try
                    {
                        while (_reader.Read() && !nextLoop)
                        {
                            if (_reader.IsStartElement())
                            {
                                switch (_reader.Name)
                                {
                                    case "product":
                                        p = new Product();
                                        p.AffiliateProdID = _reader.GetAttribute("ID");
                                        break;

                                    case "name":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "price":
                                        p.Currency = _reader.GetAttribute("currency");
                                        _reader.Read();
                                        p.Price = _reader.Value;
                                        break;

                                    case "URL":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "image":
                                        _reader.Read();
                                        p.Image_Loc = _reader.Value;
                                        break;

                                    case "description":
                                        _reader.Read();
                                        p.Description = _reader.Value;
                                        break;

                                    case "category":
                                        _reader.Read();
                                        p.Category = _reader.Value;
                                        break;

                                    case "property":
                                        if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                        switch (_reader.Value)
                                        {
                                            case "currency":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Currency = _reader.Value;
                                                break;

                                            case "color":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Title += Regex.IsMatch(@"^[a-zA-Z\s]+$", _reader.Value) ? " " + _reader.Value : "";
                                                break;

                                            case "EAN":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.EAN = _reader.Value;
                                                break;

                                            case "brand":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Brand = _reader.Value;
                                                break;

                                            case "deliveryCosts":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.DeliveryCost = _reader.Value;
                                                break;

                                            case "stock":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Stock = _reader.Value;
                                                break;

                                            case "deliveryTime":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.DeliveryTime = _reader.Value;
                                                break;

                                            case "SKU":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.SKU = _reader.Value;
                                                break;

                                            case "phonePrice":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Price = _reader.Value;
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
                                p.Webshop = fileUrl;
                                products.Add(p);
                            }

                            nextLoop = products.Count >= PackageSize;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        Console.WriteLine("From producer: Thread was aborted. Shutting down.");
                    }
                    catch (XmlException xmle)
                    {
                        Logger.Instance.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                    }

                    isDone = !nextLoop;
                    nextLoop = false;
                    yield return products;
                    products.Clear();
                }
                yield return products;
            }
        }

        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Directory not found: " + dir);
                yield break;
            }

            Console.WriteLine("Started reading from: " + dir);

            List<Product> products = new List<Product>();
            string[] filePaths = Util.ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

            foreach (string file in filePaths)
            {
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                // If the webshop is not found in the webshop list no further processing needed.
                if (!Lookup.WebshopLookup.Contains(fileUrl))
                {
                    /*using (Logger logger = new Logger(Statics.LoggerPath, true))
                    {
                        logger.WriteLine("Webshop not found in database: " + fileUrl + " from " + Name);
                    }*/
                    continue;
                }

                XmlReader _reader = XmlReader.Create(file);
                Product p = null;
                while (_reader.Read())
                {
                    if (products.Count > PackageSize)
                    {
                        yield return products;
                        products.Clear();
                    }
                    try
                    {
                        if (_reader.IsStartElement())
                        {
                            switch (_reader.Name)
                            {
                                case "product":
                                    p = new Product();
                                    p.AffiliateProdID = _reader.GetAttribute("ID");
                                    break;

                                case "name":
                                    _reader.Read();
                                    p.Title = _reader.Value;
                                    break;

                                case "price":
                                    p.Currency = _reader.GetAttribute("currency");
                                    _reader.Read();
                                    p.Price = _reader.Value;
                                    break;

                                case "URL":
                                    _reader.Read();
                                    p.Url = _reader.Value;
                                    break;

                                case "image":
                                    _reader.Read();
                                    p.Image_Loc = _reader.Value;
                                    break;

                                case "description":
                                    _reader.Read();
                                    p.Description = _reader.Value;
                                    break;

                                case "category":
                                    _reader.Read();
                                    p.Category = _reader.Value;
                                    break;

                                case "property":
                                    if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                    switch (_reader.Value)
                                    {
                                        case "currency":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.Currency = _reader.Value;
                                            break;

                                        case "color":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.Title += Regex.IsMatch(@"^[a-zA-Z\s]+$", _reader.Value) ? " " + _reader.Value : "";
                                            break;

                                        case "EAN":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.EAN = _reader.Value;
                                            break;

                                        case "brand":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.Brand = _reader.Value;
                                            break;

                                        case "deliveryCosts":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.DeliveryCost = _reader.Value;
                                            break;

                                        case "stock":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.Stock = _reader.Value;
                                            break;

                                        case "deliveryTime":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.DeliveryTime = _reader.Value;
                                            break;

                                        case "SKU":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.SKU = _reader.Value;
                                            break;

                                        case "phonePrice":
                                            _reader.Read();
                                            _reader.Read();
                                            _reader.Read();
                                            p.Price = _reader.Value;
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
                            p.Webshop = fileUrl;
                            products.Add(p);
                        }
                    }

                    catch (ThreadAbortException)
                    {
                        Console.WriteLine("From producer: Thread was aborted. Shutting down.");
                    }
                    catch (XmlException xmle)
                    {
                        Logger.Instance.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                    }
                }
                yield return products;
                products.Clear();
                _reader.Close();
            }                              
        }
    }
}
