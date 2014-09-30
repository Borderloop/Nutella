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
    public class TradeTracker : AffiliateBase
    {
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

            //Initialize XmlValueReader and its keys
            XmlValueReader xvr = new XmlValueReader();
            xvr.ProductEnd = "product";
            xvr.AddKeys("EAN", XmlNodeType.Attribute);
            xvr.AddKeys("SKU", XmlNodeType.Attribute);
            xvr.AddKeys("name", XmlNodeType.Element);
            xvr.AddKeys("brand", XmlNodeType.Attribute);
            xvr.AddKeys("price", XmlNodeType.Element);
            xvr.AddKeys("currency", XmlNodeType.Attribute);
            xvr.AddKeys("URL", XmlNodeType.Element);
            xvr.AddKeys("image", XmlNodeType.Element);
            xvr.AddKeys("category", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("deliveryCosts", XmlNodeType.Attribute);
            xvr.AddKeys("stock", XmlNodeType.Attribute);
            xvr.AddKeys("deliveryTime", XmlNodeType.Attribute);
            xvr.AddKeys("TDProductId", XmlNodeType.Element);

            Product p = new Product();

            foreach (string file in filePaths)
            {
                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    //Fill the product with fields
                    p.EAN = Regex.IsMatch(dkd["EAN"][XmlNodeType.Attribute].EmptyNull(), @"^[0-9]{10,13}$") ? dkd["EAN"][XmlNodeType.Attribute] : ""; 
                    p.SKU = dkd["SKU"][XmlNodeType.Attribute];
                    p.Title = dkd["name"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Attribute];
                    p.Price = dkd["price"][XmlNodeType.Element];
                    p.Url = dkd["URL"][XmlNodeType.Element];
                    p.Image_Loc = dkd["image"][XmlNodeType.Element];
                    p.Category = dkd["category"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["deliveryCosts"][XmlNodeType.Attribute];
                    p.DeliveryTime = dkd["deliveryTime"][XmlNodeType.Attribute];
                    p.Stock = dkd["stock"][XmlNodeType.Attribute];
                    p.AfiiliateProdID = dkd["TDProductId"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Attribute];
                    p.Affiliate = "TradeTracker";
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

