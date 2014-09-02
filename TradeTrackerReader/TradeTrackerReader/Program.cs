using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TradeTrackerReader
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Product> products = 
                (
                    from e in XDocument.Load(@"C:\productfeed.xml").Root.Elements("product")
                    select new Product
                    {
                        ID = (string)e.Attribute("ID"),
                        Name = (string)e.Element("name"),
                        Url = (string)e.Element("URL"),
                        Image = (string)e.Element("images").Element("image"),
                        Description = (string)e.Element("description"),
                        Category = (string)e.Element("categories").Element("category"),
                        SKU = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "SKU").Value,
                        DeliveryCost = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "deliveryCosts").Value,
                        DeliveryTime = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "deliveryTime").Value,
                        EAN = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "EAN").Value,
                        Stock = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "stock").Value,
                        Brand = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "brand").Value,
                        Color = (string)e.Element("properties").Elements("property").First(x => x.HasAttributes && x.Attribute("name").Value == "color").Value

                    }).ToList();

            Console.WriteLine(products[500].toString());
            Console.Read();
        }

    }
}
