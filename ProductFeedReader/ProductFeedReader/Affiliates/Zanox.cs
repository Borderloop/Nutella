﻿using System;
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
    public class Zanox : AffiliateBase
    {
        public override string Name { get { return "Zanox"; } }

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
                    string lastUpdated = null;
                    string currency = null;
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
                                case "streamCurrency":
                                    if (_reader.Read())
                                    {
                                        currency = _reader.Value;
                                    }
                                    break;

                                case "lastUpdated":
                                    if (_reader.Read())
                                    {
                                        lastUpdated = _reader.Value;
                                    }
                                    break;

                                case "record":
                                    p = new Product();
                                    break;

                                case "column":
                                    if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                    switch (_reader.Value)
                                    {
                                        case "url":
                                            if (_reader.Read())
                                            {
                                                p.Url = _reader.Value;
                                            }
                                            break;

                                        case "title":
                                            if(_reader.Read())
                                            {
                                                p.Name = _reader.Value;
                                            }
                                            break;

                                        case "ean":
                                            if (_reader.Read())
                                            {
                                                p.EAN = Regex.IsMatch(_reader.Value, @"^[0-9]{10,13}$") ? _reader.Value : "";
                                            }
                                            break;

                                        case "price":
                                            if (_reader.Read())
                                            {
                                                p.Price = _reader.Value;
                                            }
                                            break;

                                        case "image":
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

                                        case "price_shipping":
                                            if (_reader.Read())
                                            {
                                                p.DeliveryCost = _reader.Value;
                                            }
                                            break;

                                        case "stock":
                                            if (_reader.Read())
                                            {
                                                p.Stock = _reader.Value;
                                            }
                                            break;

                                        case "timetoship":
                                            if (_reader.Read())
                                            {
                                                p.DeliveryTime = _reader.Value;
                                            }
                                            break;                                                      
                                    }
                                    _reader.MoveToElement();
                                    break;
                            }
                        }

                        if (_reader.Name.Equals("record") && _reader.NodeType == XmlNodeType.EndElement)
                        {
                                        
                            p.Currency = currency;
                            p.LastModified = lastUpdated;
                            p.Affiliate = "Zanox";
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