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
    public class TradeDoubler : AffiliateBase
    {
        public override string Name { get { return "TradeDoubler"; } }

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
            xvr.ProductEnd = "product";
            xvr.AddKeys("ean", XmlNodeType.Element);
            xvr.AddKeys("sku", XmlNodeType.Element);
            xvr.AddKeys("name", XmlNodeType.Element);
            xvr.AddKeys("brand", XmlNodeType.Element);
            xvr.AddKeys("price", XmlNodeType.Element);
            xvr.AddKeys("currency", XmlNodeType.Element);
            xvr.AddKeys("productUrl", XmlNodeType.Element);
            xvr.AddKeys("imageUrl", XmlNodeType.Element);
            xvr.AddKeys("TDCategoryName", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("shippingCost", XmlNodeType.Element);
            xvr.AddKeys("inStock", XmlNodeType.Element);
            xvr.AddKeys("deliveryTime", XmlNodeType.Element);
            xvr.AddKeys("TDProductId", XmlNodeType.Element);

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
                                    case "ean":
                                        _reader.Read();
                                        p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
                                        break;

                                    case "name":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "brand":
                                        _reader.Read();
                                        p.Brand = _reader.Value;
                                        break;

                                    case "price":
                                        _reader.Read();
                                        p.Price = _reader.Value;
                                        break;

                                    case "currency":
                                        _reader.Read();
                                        p.Currency = _reader.Value;
                                        break;

                                    case "productUrl":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "imageUrl":
                                        _reader.Read();
                                        p.Image_Loc = _reader.Value;
                                        break;

                                    case "TDCategoryName":
                                        _reader.Read();
                                        p.Category = _reader.Value;
                                        break;

                                    case "description":
                                        _reader.Read();
                                        p.Description = _reader.Value;
                                        break;

                                    case "shippingCost":
                                        _reader.Read();
                                        p.DeliveryCost = _reader.Value;
                                        break;

                                    case "deliveryTime":
                                        _reader.Read();
                                        p.DeliveryTime = _reader.Value;
                                        break;

                                    case "inStock":
                                        _reader.Read();
                                        p.Stock = _reader.Value;
                                        break;

                                    case "sku":
                                        _reader.Read();
                                        p.SKU = _reader.Value;
                                        break;

                                    case "TDProductId":
                                        _reader.Read();
                                        p.AfiiliateProdID = _reader.Value;
                                        break;

                                    case "product":
                                        p = new Product();
                                        break;
                                }
                            }

                            if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                            {
                                p.Affiliate = "TradeDoubler";
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
                    //Fill the product with fields
                    p.EAN = dkd["ean"][XmlNodeType.Element]; 
                    p.SKU = dkd["sku"][XmlNodeType.Element];
                    p.Title = dkd["name"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Element];
                    p.Price = dkd["price"][XmlNodeType.Element];
                    p.Url = dkd["productUrl"][XmlNodeType.Element];
                    p.Image_Loc = dkd["imageUrl"][XmlNodeType.Element];
                    p.Category = dkd["TDCategoryName"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["shippingCost"][XmlNodeType.Element];
                    p.DeliveryTime = dkd["deliveryTime"][XmlNodeType.Element];
                    p.Stock = dkd["inStock"][XmlNodeType.Element];
                    p.AfiiliateProdID = dkd["TDProductId"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Affiliate = "TradeDoubler";
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
