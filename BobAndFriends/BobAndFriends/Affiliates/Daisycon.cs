using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;

namespace BobAndFriends.Affiliates
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by Webgains.
    /// The reading is automated by using the XmlValueReader.
    /// </summary>
    public class Daisycon : AffiliateBase
    {
        public override string Name { get { return "Daisycon"; } }

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

            //Initialize XmlValueReader and its keys
            XmlValueReader xvr = new XmlValueReader();
            xvr.ProductEnd = "item";
            xvr.AddKeys("ean_code", XmlNodeType.Element);
            xvr.AddKeys("title", XmlNodeType.Element);
            xvr.AddKeys("brand", XmlNodeType.Element);
            xvr.AddKeys("minimum_price", XmlNodeType.Element);
            xvr.AddKeys("link", XmlNodeType.Element);
            xvr.AddKeys("img_medium", XmlNodeType.Element);
            xvr.AddKeys("category", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("shipping_cost", XmlNodeType.Element);
            xvr.AddKeys("shippingcost", XmlNodeType.Element);
            xvr.AddKeys("stock", XmlNodeType.Element);
            xvr.AddKeys("shipping_duration_descr", XmlNodeType.Element);
            xvr.AddKeys("shipping_duration", XmlNodeType.Element);
            xvr.AddKeys("daisycon_unique_id", XmlNodeType.Element);

            Product p = new Product();

            foreach (string file in filePaths)
            {
<<<<<<< HEAD
                //First check if the website is in the database. If not, log it and if so, proceed.
                string urlLine;
                bool websitePresent = false;
                System.IO.StreamReader urlTxtFile = new System.IO.StreamReader("C:\\BorderSoftware\\BOBAndFriends\\weburls.txt");

                //Read all lines from the urlTxtFile.
                while ((urlLine = urlTxtFile.ReadLine()) != null)
                {
                    if (urlLine == Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/'))// Found a similar website
                    {
                        websitePresent = true;
                        break;
                    }
                }
                // If websitePresent == false, the webshop is not found in the webshop list. No further processing needed.
                if (websitePresent == false)
                {
                    Statics.Logger.WriteLine("Webshop not found in database: " + Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/'));
                }
                else
                {
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
                                    case "ean_code":
                                        _reader.Read();
                                        p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
                                        break;

                                    case "title":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "brand":
                                        _reader.Read();
                                        p.Brand = _reader.Value;
                                        break;

                                    case "minimum_price":
                                        _reader.Read();
                                        p.Price = _reader.Value;
                                        break;

                                    case "link":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "img_medium":
                                        _reader.Read();
                                        p.Image_Loc = _reader.Value;
                                        break;

                                    case "category":
                                        _reader.Read();
                                        p.Category = _reader.Value;
                                        break;

                                    case "description":
                                        _reader.Read();
                                        p.Description = _reader.Value;
                                        break;

                                    case "shipping_cost":
                                    case "shippingcosts":
                                        _reader.Read();
                                        p.DeliveryCost = _reader.Value;
                                        break;

                                    case "shipping_duration":
                                        _reader.Read();
                                        p.DeliveryTime = _reader.Value;
                                        break;

                                    case "stock":
                                    case "shipping_duration_descr":
                                        _reader.Read();
                                        p.Stock = _reader.Value;
                                        break;

                                    case "daisycon_unique_id":
                                        _reader.Read();
                                        p.AfiiliateProdID = _reader.Value;
                                        break;

                                    case "item":
                                        p = new Product();
                                        break;
                                }
                            }

                            if (_reader.Name.Equals("item") && _reader.NodeType == XmlNodeType.EndElement)
                            {
                                p.Currency = "EUR";
                                p.Affiliate = "Daisycon";
                                p.FileName = file;
                                p.Webshop = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                                products.Add(p);
                            }
                        }
                    }
                    catch (XmlException xmle)
                    {
                        Statics.Logger.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                    }
                    catch (Exception e)
                    {
                        Statics.Logger.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                    }
                    yield return products;
                    products.Clear();
                }
            }
        }
    }
}
=======
                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["ean_code"][XmlNodeType.Element]; 
                    p.Title = dkd["title"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Element];
                    p.Price = dkd["minimum_price"][XmlNodeType.Element];
                    p.Url = dkd["link"][XmlNodeType.Element];
                    p.Image_Loc = dkd["img_medium"][XmlNodeType.Element];
                    p.Category = dkd["category"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["shipping_cost"][XmlNodeType.Element] == default(string) ? dkd["shippingcost"][XmlNodeType.Element] : dkd["shipping_duration"][XmlNodeType.Element];
                    p.DeliveryTime = dkd["shipping_duration"][XmlNodeType.Element];
                    p.Stock = dkd["stock"][XmlNodeType.Element] == default(string) ? dkd["shipping_duration_descr"][XmlNodeType.Element] : dkd["stock"][XmlNodeType.Element];
                    p.AfiiliateProdID = dkd["daisycon_unique_id"][XmlNodeType.Element];
                    p.Currency = "EUR";
                    p.Affiliate = "Daisycon";
                    p.FileName = file;
                    p.Webshop = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                    products.Add(p);
                    p = new Product();
                }
            }
            yield return products;
            products.Clear();
        }
    }
}
    
>>>>>>> 9fa828420c9ef33f5cde6400d5dc76e5613c4aee
