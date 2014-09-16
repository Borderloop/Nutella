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
    public class Daisycon : AffiliateBase
    {
        public override string Name { get { return "Daisycon"; } }

        public override System.Collections.Generic.IEnumerable<List<Product>>ReadFromDir(string dir)
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
                                case "ean_code":
                                    if (_reader.Read())
                                    {
                                        p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
                                    }
                                    break;

                                case "title":
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

                                case "minimum_price":
                                    if (_reader.Read())
                                    {
                                        p.Price = _reader.Value;
                                    }
                                    break;                                            

                                case "link":
                                    if (_reader.Read())
                                    {
                                        p.Url = _reader.Value;
                                    }
                                    break;

                                case "img_medium":
                                    if (_reader.Read())
                                    {
                                        p.Image_Loc = _reader.Value;
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

                                case "shipping_cost":
                                case "shippingcosts":
                                    if (_reader.Read())
                                    {
                                        p.DeliveryCost = _reader.Value;
                                    }
                                    break;

                                case "shipping_duration":
                                    if (_reader.Read())
                                    {
                                        p.DeliveryTime = _reader.Value;
                                    }
                                    break;

                                case "stock":
                                case "shipping_duration_descr":
                                    if (_reader.Read())
                                    {
                                        p.Stock = _reader.Value;
                                    }
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
