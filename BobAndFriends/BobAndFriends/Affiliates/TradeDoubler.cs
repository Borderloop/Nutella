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

            foreach (string file in filePaths)
            {
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