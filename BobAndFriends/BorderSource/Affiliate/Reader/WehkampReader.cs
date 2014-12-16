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

namespace BorderSource.Affiliate.Reader
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by WebgainsReader.
    /// The reading is automated by using the XmlValueReader.
    /// </summary>
    public class WehkampReader : AffiliateReaderBase
    {

        public override string Name { get { return "Wehkamp"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

            using (XmlReader _reader = XmlReader.Create(file, new XmlReaderSettings { CloseInput = true }))
            {
                List<Product> products = new List<Product>();
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
                                    case "ns2:ean":
                                        _reader.Read();
                                        p.EAN = _reader.Value;
                                        break;

                                    case "ns2:brand":
                                        _reader.Read();
                                        p.Brand = _reader.Value;
                                        break;

                                    case "ns2:name":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "ns2:productUrl":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "ns2:productImage":
                                        _reader.Read();
                                        p.Image_Loc = _reader.Value;
                                        break;

                                    case "ns2:description":
                                        _reader.Read();
                                        p.Description = _reader.Value;
                                        break;

                                    case "ns2:availability":
                                        _reader.Read();
                                        p.Stock = _reader.Value;
                                        break;

                                    case "ns2:deliveryTime":
                                        _reader.Read();
                                        p.DeliveryTime = _reader.Value;
                                        break;

                                    case "ns2:shippingCost":
                                        _reader.Read();
                                        p.DeliveryCost = _reader.Value;
                                        break;

                                    case "ns2:field":
                                        if (_reader.HasAttributes && _reader["name"].Equals("retailPrice"))
                                        {
                                            _reader.Read();
                                            p.Price = _reader.Value;
                                            break;
                                        }
                                        if (_reader.HasAttributes && _reader["name"].Equals("ShopOmschrijving"))
                                        {
                                            _reader.Read();
                                            p.Category = _reader.Value;
                                            break;
                                        }
                                        break;

                                    case "offer":
                                        if (_reader.HasAttributes)
                                            p.AffiliateProdID = _reader["id"];
                                        break;

                                    case "product":
                                        p = new Product();
                                        break;
                                }
                            }

                            if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                            {
                                p.Affiliate = "Wehkamp";
                                p.FileName = file;
                                p.Webshop = "www.wehkamp.nl";
                                products.Add(p);
                            }

                            nextLoop = products.Count >= PackageSize;
                        }

                    }

                    catch (ThreadAbortException)
                    {
                        Console.WriteLine("From producer: Thread was aborted. Shutting down.");
                        yield break;
                    }
                    catch (XmlException xmle)
                    {
                        Logger.Instance.WriteLine("BAD XML FILE: " + file + " ERROR: " + xmle.Message);
                        yield break;
                    }
                    isDone = !nextLoop;
                    nextLoop = false;
                    yield return products;
                    products.Clear();

                }
                yield return products;
            }
        }


        [Obsolete]
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
                        logger.WriteLine("Webshop not found in database: " + fichierUrl + " from " + Name) ;
                    }*/
                    continue;
                }


                using (XmlReader _reader = XmlReader.Create(file, new XmlReaderSettings { CloseInput = true }))
                {
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
                                        case "ns2:ean":
                                            _reader.Read();
                                            p.EAN = _reader.Value;
                                            break;

                                        case "ns2:name":
                                            _reader.Read();
                                            p.Title = _reader.Value;
                                            break;

                                        case "Brand":
                                            _reader.Read();
                                            p.Brand = _reader.Value;
                                            break;

                                        case "ns2:productUrl":
                                            _reader.Read();
                                            p.Url = _reader.Value;
                                            break;

                                        case "ns2:productImage":
                                            _reader.Read();
                                            p.Image_Loc = _reader.Value;
                                            break;

                                        case "ns2:description":
                                            _reader.Read();
                                            p.Description = _reader.Value;
                                            break;

                                        case "ns2:availability":
                                            _reader.Read();
                                            p.Stock = _reader.Value;
                                            break;

                                        case "ns2:deliveryTime":
                                            _reader.Read();
                                            p.DeliveryTime = _reader.Value;
                                            break;

                                        case "ns2:shippingCost":
                                            _reader.Read();
                                            p.DeliveryCost = _reader.Value;
                                            break;

                                        case "ns2:field":
                                            if (_reader.HasAttributes && _reader["name"].Equals("retailPrice"))
                                            {
                                                _reader.Read();
                                                p.Price = _reader.Value;
                                                break;
                                            }
                                            if (_reader.HasAttributes && _reader["name"].Equals("ShopOmschrijving"))
                                            {
                                                _reader.Read();
                                                p.Category = _reader.Value;
                                                break;
                                            }
                                            break;

                                        case "offer":
                                            if (_reader.HasAttributes)
                                                p.AffiliateProdID = _reader["id"];
                                            break;

                                        case "product":
                                            p = new Product();
                                            break;
                                    }
                                }

                                if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                                {
                                    p.Affiliate = "Wehkamp";
                                    p.FileName = file;
                                    p.Webshop = "www.wehkamp.nl";
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
        }
    }
}