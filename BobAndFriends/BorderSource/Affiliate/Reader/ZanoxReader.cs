﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using BorderSource.Common;
using BorderSource.ProductAssociation;
using BorderSource.Loggers;

namespace BorderSource.Affiliate.Reader
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by ZanoxReader.
    /// This reading cannot be automated with the XmlValueReader because the xml
    /// is delivered too complicated.
    /// </summary>
    public class ZanoxReader : AffiliateReaderBase
    {

        public override string Name { get { return "Zanox"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            using (XmlReader _reader = XmlReader.Create(file, new XmlReaderSettings { CloseInput = true }))
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                Product p = new Product();
                string lastUpdated = null;
                string currency = null;
                bool isDone = false;
                bool nextLoop = false;
                while (!isDone)
                {
                    try
                    {
                        while (_reader.Read() && !nextLoop)
                        {
                            if (_reader.IsStartElement())
                            {
                                if (_reader.IsEmptyElement) continue;
                                switch (_reader.Name)
                                {
                                    case "streamCurrency":
                                        _reader.Read();
                                        currency = _reader.Value;
                                        break;

                                    case "lastUpdated":
                                        _reader.Read();
                                        lastUpdated = _reader.Value;
                                        break;

                                    case "record":
                                        p = new Product();
                                        break;

                                    case "product":
                                        p = new Product();
                                        break;

                                    case "url":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "deepLink":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "title":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "name":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "ean":
                                        _reader.Read();
                                        p.EAN = _reader.Value;
                                        break;

                                    case "price":
                                        _reader.Read();
                                        p.Price = _reader.Value;
                                        break;

                                    case "image":
                                        _reader.Read();
                                        p.Image_Loc = _reader.Value;
                                        break;

                                    case "largeImage":
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

                                    case "price_shipping":
                                        _reader.Read();
                                        p.DeliveryCost = _reader.Value;
                                        break;

                                    case "shippingHandlingCost":
                                        _reader.Read();
                                        p.DeliveryCost = _reader.Value;
                                        break;

                                    case "stock":
                                        _reader.Read();
                                        p.Stock = _reader.Value;
                                        break;

                                    case "timetoship":
                                        _reader.Read();
                                        p.DeliveryTime = _reader.Value;
                                        break;

                                    case "deliveryTime":
                                        _reader.Read();
                                        p.DeliveryTime = _reader.Value;
                                        break;

                                    case "manufacturer":
                                        _reader.Read();
                                        p.Brand = _reader.Value;
                                        break;

                                    case "currencyCode":
                                        _reader.Read();
                                        p.Currency = _reader.Value;
                                        break;

                                    case "column":
                                        if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                        switch (_reader.Value)
                                        {
                                            case "url":
                                                _reader.Read();
                                                p.Url = _reader.Value;
                                                break;

                                            case "title":
                                                _reader.Read();
                                                p.Title = _reader.Value;
                                                break;

                                            case "ean":
                                                _reader.Read();
                                                p.EAN = _reader.Value;
                                                break;

                                            case "price":
                                                _reader.Read();
                                                p.Price = _reader.Value;
                                                break;

                                            case "image":
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

                                            case "price_shipping":
                                                _reader.Read();
                                                p.DeliveryCost = _reader.Value;
                                                break;

                                            case "stock":
                                                _reader.Read();
                                                p.Stock = _reader.Value;
                                                break;

                                            case "timetoship":
                                                _reader.Read();
                                                p.DeliveryTime = _reader.Value;
                                                break;

                                        }
                                        _reader.MoveToElement();
                                        break;
                                }
                            }

                            if ((_reader.Name.Equals("record") || _reader.Name.Equals("product")) && _reader.NodeType == XmlNodeType.EndElement)
                            {
                                p.Currency = currency;
                                p.AffiliateProdID = p.Url.ToSHA256();
                                p.LastModified = lastUpdated;
                                p.Affiliate = "Zanox";
                                p.FileName = file;
                                p.Webshop = fileUrl;
                                products.Add(p);
                            }

                            nextLoop = products.Count >= PackageSize;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        Console.WriteLine("From producer: Thread was aborted. Shutting down.");
                        yield break;
                    }
                    catch (XmlException xmle)
                    {
                        Logger.Instance.WriteLine("BAD XML FILE: " + file + " ERROR: " + xmle.Message);
                        yield break;
                    }
                    isDone = !nextLoop;
                    nextLoop = false;
                    yield return products;
                    products.Clear();
                }
                yield return products;
            }
        }

        [Obsolete]
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

                XmlReader _reader = XmlReader.Create(file);
                Product p = null;
                string lastUpdated = null;
                string currency = null;
                while (_reader.Read())
                {
                    if (products.Count > PackageSize)
                    {
                        yield return products;
                        products.Clear();
                    }
                    try
                    {
                        if (_reader.IsStartElement())
                        {
                            switch (_reader.Name)
                            {
                                case "streamCurrency":
                                    _reader.Read();
                                    currency = _reader.Value;
                                    break;

                                case "lastUpdated":
                                    _reader.Read();
                                    lastUpdated = _reader.Value;
                                    break;

                                case "record":
                                    p = new Product();
                                    break;

                                case "column":
                                    if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                    switch (_reader.Value)
                                    {
                                        case "url":
                                            _reader.Read();
                                            p.Url = _reader.Value;
                                            break;

                                        case "title":
                                            _reader.Read();
                                            p.Title = _reader.Value;
                                            break;

                                        case "ean":
                                            _reader.Read();
                                            p.EAN = _reader.Value;
                                            break;

                                        case "price":
                                            _reader.Read();
                                            p.Price = _reader.Value;
                                            break;

                                        case "image":
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

                                        case "price_shipping":
                                            _reader.Read();
                                            p.DeliveryCost = _reader.Value;
                                            break;

                                        case "stock":
                                            _reader.Read();
                                            p.Stock = _reader.Value;
                                            break;

                                        case "timetoship":
                                            _reader.Read();
                                            p.DeliveryTime = _reader.Value;
                                            break;

                                        case "zupid":
                                            _reader.Read();
                                            p.AffiliateProdID = _reader.Value;
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
                            p.Webshop = fileUrl;
                            products.Add(p);
                        }
                    }

                    catch (ThreadAbortException)
                    {
                        Console.WriteLine("From producer: Thread was aborted. Shutting down.");
                    }
                    catch (XmlException xmle)
                    {
                        Logger.Instance.WriteLine("BAD XML FILE: " + file + " ### ERROR: " + xmle.Message + " ###");
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.WriteLine("BAD FILE: " + file + " ### ERROR: " + e.Message + " ###");
                    }
                }
                yield return products;
                products.Clear();
                _reader.Close();
            }
        }
    }
}
