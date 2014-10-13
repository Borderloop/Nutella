using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using BorderSource.Common;

namespace BobAndFriends.Affiliates
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by Zanox.
    /// This reading cannot be automated with the XmlValueReader because the xml
    /// is delivered too complicated.
    /// </summary>
    public class TradeTracker : AffiliateBase
    {
        // Stores the name of the website that is being processed.
        private string _fileUrl;

        public override string Name { get { return "TradeTracker"; } }

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
                _fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                System.IO.StreamReader urlTxtFile = new System.IO.StreamReader("C:\\BorderSoftware\\BOBAndFriends\\weburls.txt");

                //Read all lines from the urlTxtFile.
                while ((urlLine = urlTxtFile.ReadLine()) != null)
                {
                    if (urlLine == _fileUrl)// Found a similar website
                    {
                        websitePresent = true;
                        break;
                    }
                }                
                // If websitePresent == false, the webshop is not found in the webshop list. No further processing needed.
                if (websitePresent == false)
                {
                    Statics.Logger.WriteLine("Webshop not found in database: " + _fileUrl);
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
                                    case "product":
                                        p = new Product();
                                        p.AffiliateProdID = _reader.GetAttribute("ID");
                                        break;

                                    case "name":
                                        _reader.Read();
                                        p.Title = _reader.Value;
                                        break;

                                    case "price":
                                        p.Currency = _reader.GetAttribute("currency");
                                        _reader.Read();
                                        p.Price = _reader.Value;
                                        break;

                                    case "URL":
                                        _reader.Read();
                                        p.Url = _reader.Value;
                                        break;

                                    case "image":
                                        _reader.Read();
                                        p.Image_Loc = _reader.Value;
                                        break;

                                    case "description":
                                        _reader.Read();
                                        p.Description = _reader.Value;
                                        break;

                                    case "category":
                                        _reader.Read();
                                        p.Category = _reader.Value;
                                        break;

                                    case "property":
                                        if (_reader.HasAttributes) { _reader.MoveToNextAttribute(); }
                                        switch (_reader.Value)
                                        {
                                            case "currency":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Currency = _reader.Value;
                                                break;

                                            case "color":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Title += Regex.IsMatch(@"^[a-zA-Z\s]+$", _reader.Value) ?  " " + _reader.Value : "";
                                                break;

                                            case "EAN":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.EAN = _reader.Value;
                                                break;

                                            case "brand":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Brand = _reader.Value;
                                                break;

                                            case "deliveryCosts":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.DeliveryCost = _reader.Value;
                                                break;

                                            case "stock":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.Stock = _reader.Value;
                                                break;

                                            case "deliveryTime":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.DeliveryTime = _reader.Value;
                                                break;

                                            case "SKU":
                                                _reader.Read();
                                                _reader.Read();
                                                _reader.Read();
                                                p.SKU = _reader.Value;
                                                break;
                                        }
                                        _reader.MoveToElement();
                                        break;
                                }
                            }

                            if (_reader.Name.Equals("product") && _reader.NodeType == XmlNodeType.EndElement)
                            {
                                p.Affiliate = "TradeTracker";
                                p.FileName = file;
                                p.Webshop = _fileUrl;
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
