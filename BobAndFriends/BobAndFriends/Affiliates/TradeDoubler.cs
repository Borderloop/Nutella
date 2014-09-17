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

            List<Product> products = new List<Product>();
            string[] filePaths = Util.ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

            foreach (string file in filePaths)
            {
                Console.Write("Started reading from: " + file + " ...");
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
                                    if (_reader.Read())
                                    {
                                        p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
                                    }
                                    break;

                                case "name":
                                    if (_reader.Read())
                                    {
                                        p.Title = _reader.Value;
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

                                case "productUrl":
                                    if (_reader.Read())
                                    {
                                        p.Url = _reader.Value;
                                    }
                                    break;

                                case "imageUrl":
                                    if (_reader.Read())
                                    {
                                        p.Image_Loc = _reader.Value;
                                    }
                                    break;

                                case "TDCategoryName":
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

                                case "shippingCost":
                                    if (_reader.Read())
                                    {
                                        p.DeliveryCost = _reader.Value;
                                    }
                                    break;

                                case "deliveryTime":
                                    if (_reader.Read())
                                    {
                                        p.DeliveryTime = _reader.Value;
                                    }
                                    break;

                                case "inStock":
                                    if (_reader.Read())
                                    {
                                        p.Stock = _reader.Value;
                                    }
                                    break;

                                case "sku":
                                    if (_reader.Read())
                                    {
                                        p.SKU = _reader.Value;
                                    }
                                    break;

                                case "product":
                                    p = new Product();
                                    break;
                            }
                        }

                        if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                        {
                            p.Affiliate = "Tradedoubler";
                            p.FileName = file;
                            p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
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
                Console.WriteLine(" Done");
                yield return products;
                products.Clear();                
            }
        }
        }
    }
