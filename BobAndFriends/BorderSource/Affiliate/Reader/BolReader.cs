using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using BorderSource.ProductAssociation;

namespace BorderSource.Affiliate.Reader
{
    public class BolReader : AffiliateReaderBase
    {
        private string SiteId = "31102";
        private string SubId = DateTime.Today.ToString("yyyyMMdd");
        private string BolName = "Borderloop";
        public override string Name
        {
            get { return "Bol"; }
        }

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tried to access BolReader from ReadFromDir(). This is not implemented because it's obsolete.");
            yield break;
        }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            using (var sr = new StreamReader(file))
            {
                sr.ReadLine();
                using (CsvReader reader = new CsvReader(sr, false, '|'))
                {
                    List<Product> products = new List<Product>();
                    switch (Path.GetFileNameWithoutExtension(file).Split('_')[1])
                    {
                        case "muziek":
                            yield break;
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[14],
                                    Brand = reader[8],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[10],
                                    DeliveryTime = reader[11],
                                    EAN = reader[13],
                                    FileName = file,
                                    Image_Loc = reader[15],
                                    Price = reader[9],
                                    Stock = reader[7],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s="+ SiteId + "&url="+ reader[16] +"&f=TXL&subid="+ SubId + "&name="+ BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;                      
                        case "baby":
                            yield break;
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[15],
                                    Brand = reader[9],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[11],
                                    DeliveryTime = reader[12],
                                    EAN = reader[15],
                                    FileName = file,
                                    Image_Loc = reader[16],
                                    Price = reader[10],
                                    Stock = reader[7],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[17] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                        case "dvd":
                            yield break;
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[15],
                                    Brand = reader[8],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[11],
                                    DeliveryTime = reader[12],
                                    EAN = reader[14],
                                    FileName = file,
                                    Image_Loc = reader[16],
                                    Price = reader[10],
                                    Stock = reader[14],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[17] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                        case "boeken-engels":
                        case "boeken":
                            yield break;
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[16],
                                    Brand = reader[9],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[11],
                                    DeliveryTime = reader[12],
                                    EAN = reader[15],
                                    FileName = file,
                                    Image_Loc = reader[17],
                                    Price = reader[10],
                                    Stock = reader[14],
                                    Title = reader[6],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[18] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                        case "wonen":
                        case "vrije-tijd":
                        case "speelgoed":
                        case "koken-tafelen": 
                        case "dier-tuin-klussen":
                        case "mooi-gezond":
                        case "sieraden-horloges-accessoires":
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[16],
                                    Brand = reader[9],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[11],
                                    DeliveryTime = reader[12],
                                    EAN = reader[15],
                                    FileName = file,
                                    Image_Loc = reader[17],
                                    Price = reader[10],
                                    Stock = reader[7],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[18] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                        case "mobiele-telefonie":
                          while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[18],
                                    Brand = reader[8],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[12],
                                    DeliveryTime = reader[13],
                                    EAN = reader[17],
                                    FileName = file,
                                    Image_Loc = reader[19],
                                    Price = reader[11],
                                    Stock = reader[15],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[20] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                        case "games":
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[16],
                                    Brand = reader[8],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[10],
                                    DeliveryTime = reader[11],
                                    EAN = reader[15],
                                    FileName = file,
                                    Image_Loc = reader[17],
                                    Price = reader[9],
                                    Stock = reader[13],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[18] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                        case "notebooks-pc-accessoires":
                        case "elektronica":
                            while (reader.ReadNextRecord())
                            {
                                Product p = new Product()
                                {
                                    Affiliate = "Bol",
                                    AffiliateProdID = reader[17],
                                    Brand = reader[8],
                                    Category = reader[1] + " -> " + reader[2] + " -> " + reader[3],
                                    Currency = "EUR",
                                    DeliveryCost = reader[11],
                                    DeliveryTime = reader[12],
                                    EAN = reader[16],
                                    FileName = file,
                                    Image_Loc = reader[18],
                                    Price = reader[10],
                                    Stock = reader[14],
                                    Title = reader[5],
                                    Url = "http://partnerprogramma.bol.com/click/click?p=1&t=url&s=" + SiteId + "&url=" + reader[19] + "&f=TXL&subid=" + SubId + "&name=" + BolName,
                                    Webshop = "www.bol.com"
                                };

                                products.Add(p);
                                if (products.Count >= PackageSize)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;                      
                        default: break;
                    }
                }
            }
            yield break;
        }
    }
}
