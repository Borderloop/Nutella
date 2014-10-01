﻿using System;
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

            Console.WriteLine("Started reading from: " + dir);

            List<Product> products = new List<Product>();
            string[] filePaths = Util.ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

            //Initialize XmlValueReader and its keys
            XmlValueReader xvr = new XmlValueReader();
            xvr.ProductEnd = "product";
            xvr.AddKeys("european_article_number", XmlNodeType.Element);
            xvr.AddKeys("product_name", XmlNodeType.Element);
            xvr.AddKeys("brand", XmlNodeType.Element);
            xvr.AddKeys("price", XmlNodeType.Element);
            xvr.AddKeys("currency", XmlNodeType.Element);
            xvr.AddKeys("deeplink", XmlNodeType.Element);
            xvr.AddKeys("image_url", XmlNodeType.Element);
            xvr.AddKeys("category", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("delivery_cost", XmlNodeType.Element);
            xvr.AddKeys("product_id", XmlNodeType.Element);
            xvr.AddKeys("delivery_period", XmlNodeType.Element);
            xvr.AddKeys("program_id", XmlNodeType.Element);
            xvr.AddKeys("validuntil", XmlNodeType.Element);
            xvr.AddKeys("last_updated", XmlNodeType.Element);

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
                                    case "european_article_number":
                                        _reader.Read();
                                        p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
                                        break;

                                    case "product_name":
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

                                    case "validuntil":
                                        _reader.Read();
                                        p.ValidUntil = _reader.Value;
                                        break;

                                    case "deeplink":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "image_url":
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

                                    case "last_updated":
                                        _reader.Read();
                                        p.LastModified = _reader.Value;
                                        break;

                                    case "delivery_cost":
                                        _reader.Read();
                                        p.DeliveryCost = _reader.Value;
                                        break;

                                    case "delivery_period":
                                        _reader.Read();
                                        p.DeliveryTime = _reader.Value;
                                        break;

                                    case "program_id":
                                        _reader.Read();
                                        p.AfiiliateProdID = _reader.Value;
                                        break;

                                    case "product_id":
                                        _reader.Read();

                                        //Concatinate the program id and the product id to make sure the id is unique.
                                        //Webgains doesnt provide a unique id, but this is solid enough.
                                        p.AfiiliateProdID += _reader.Value;
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
                    p.EAN = dkd["european_article_number"][XmlNodeType.Element];
                    p.Title = dkd["product_name"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Element];
                    p.Price = dkd["price"][XmlNodeType.Element];
                    p.Url = dkd["deeplink"][XmlNodeType.Element];
                    p.Image_Loc = dkd["image_url"][XmlNodeType.Element];
                    p.Category = dkd["category"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["delivery_cost"][XmlNodeType.Element];
                    p.DeliveryTime = dkd["delivery_period"][XmlNodeType.Element];
                    p.AfiiliateProdID = dkd["product_id"][XmlNodeType.Element] + dkd["program_id"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Affiliate = "Webgains";
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
