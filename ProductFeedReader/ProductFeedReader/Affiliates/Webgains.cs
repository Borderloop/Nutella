using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;

namespace ProductFeedReader.Affiliates
{
    public class Webgains : AffiliateBase
    {
        public override string Name { get { return "Webgains"; } }

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
                        if (Statics.TickCount % Statics.SleepCount == 0)
                        {
                            Thread.Sleep(1);

                            //Set tickCount to 0 to save memory
                            Statics.TickCount = 0;
                        }

                        if (_reader.IsStartElement())
                        {
                            switch (_reader.Name)
                            {
                                case "european_article_number":
                                    if (_reader.Read())
                                    {
                                        p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
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
