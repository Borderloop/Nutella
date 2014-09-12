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
    public class Belboon : AffiliateBase
    {
        public override string Name { get { return "Belboon"; } }

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
                        Util.TickCount++;

                        //Sleep everytime sleepcount is reached
                        if (Util.TickCount % Util.SleepCount == 0)
                        {
                            Thread.Sleep(1);

                            //Set tickCount to 0 to save memory
                            Util.TickCount = 0;
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
                            p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                            products.Add(p);
                        }
                    }
                }
                catch (XmlException xmle)
                {
                    Util.Logger.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                }
                catch (Exception e)
                {
                    Util.Logger.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                }
                Console.WriteLine(" Done");
                yield return products;
                products.Clear();
            }
        }
        
    }
}
