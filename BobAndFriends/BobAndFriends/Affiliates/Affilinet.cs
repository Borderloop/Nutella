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


namespace BobAndFriends.Affiliates
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by Zanox.
    /// This reading cannot be automated with the XmlValueReader because the xml
    /// is delivered too complicated.
    /// </summary>
    public class Affilinet : AffiliateBase
    {
        public override string Name { get { return "Affilinet"; } }

        public override System.Collections.Generic.IEnumerable<List<Product>> ReadFromDir(string dir)
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
                if (!Statics.webshopNames.Any(w => w == fileUrl))
                {
                    using (Logger logger = new Logger(Statics.LoggerPath))
                    {
                        logger.WriteLine("Webshop not found in database: " + fileUrl);
                    }
                    continue;
                }

                try
                {
                    XmlReader _reader = XmlReader.Create(file);
                    Product p = null;
                    while (_reader.Read())
                    {
                        //Increment the tickcount
                        Statics.TickCount++;

                        //Sleep everytime sleepcount is reached
                        if (Statics.TickCount % Statics.TicksUntilSleep == 0)
                        {
                            Thread.Sleep(1);

                            //Set tickCount to 0 to save memory
                            Statics.TickCount = 0;
                        }

                        if (_reader.IsStartElement())
                        {
                            switch (_reader.Name)
                            {
                                case "EAN":
                                    _reader.Read();
                                    p.EAN = _reader.Value;
                                    break;

                                case "Title":
                                    _reader.Read();
                                    p.Title = _reader.Value;
                                    break;

                                case "Brand":
                                    _reader.Read();
                                    p.Brand = _reader.Value;
                                    break;

                                case "Price":
                                    _reader.Read();
                                    p.Price = _reader.Value;
                                    break;

                                case "CurrencySymbol":
                                    _reader.Read();
                                    p.Currency = _reader.Value;
                                    break;

                                case "ValidTo":
                                    _reader.Read();
                                    p.ValidUntil = _reader.Value;
                                    break;

                                case "Deeplinks":
                                    _reader.Read();
                                    _reader.Read();

                                    p.Url = _reader.Value;

                                    //Read again twice to avoid double products because some genius made the xml file have two Product elements...
                                    _reader.Read();
                                    _reader.Read();
                                    break;

                                case "Images":
                                    _reader.Read();
                                    _reader.ReadToFollowing("URL");
                                    _reader.Read();
                                    p.Image_Loc = _reader.Value;
                                    break;

                                case "ProductCategoryPath":
                                    _reader.Read();
                                    p.Category = _reader.Value;
                                    break;

                                case "Description":
                                    _reader.Read();
                                    p.Description = _reader.Value;
                                    break;

                                case "LastUpdate":
                                    _reader.Read();
                                    p.LastModified = _reader.Value;
                                    break;

                                case "Shipping":
                                    _reader.Read();
                                    _reader.ReadToFollowing("Shipping");
                                    _reader.Read();
                                    p.DeliveryCost = _reader.Value;
                                    break;

                                case "Properties":
                                    _reader.Read();
                                    while (!(_reader.Name.Equals("Properties") && _reader.NodeType == XmlNodeType.EndElement))
                                    {
                                        if (_reader.HasAttributes && _reader["Title"].Equals("STOCK"))
                                        {
                                            break;
                                        }

                                        _reader.Read();

                                    }
                                    p.Stock = _reader["Text"] ?? "";
                                    break;

                                case "ProductID":
                                    _reader.Read();
                                    p.AffiliateProdID = _reader.Value;
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
                            p.Webshop = fileUrl;
                            products.Add(p);
                        }
                    }
                }

                catch (ThreadAbortException threab)
                {
                    Console.WriteLine("From producer: Thread was aborted. Shutting down.");
                }
                catch (XmlException xmle)
                {
                    using (Logger logger = new Logger(Statics.LoggerPath))
                    {
                        logger.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                    }
                }
                catch (Exception e)
                {
                    using (Logger logger = new Logger(Statics.LoggerPath))
                    {
                        logger.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                    }
                }
                yield return products;
                products.Clear();
            }
        }        
    }
}

