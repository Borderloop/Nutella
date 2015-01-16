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
using BorderSource.ProductAssociation;
using BorderSource.Loggers;

namespace BorderSource.Affiliate.Reader
{
    public class AmazonReader : AffiliateReaderBase
    {
        public override string Name { get { return "Amazon"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            // Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.ProductEnd = "Product";
                xvr.AddKeys("ean", XmlNodeType.Element);
                xvr.AddKeys("deep_link", XmlNodeType.Element);
                xvr.AddKeys("image_large", XmlNodeType.Element);
                xvr.AddKeys("ean", XmlNodeType.Element);
                xvr.AddKeys("price", XmlNodeType.Element);
                xvr.AddKeys("title", XmlNodeType.Element);
                xvr.AddKeys("currency", XmlNodeType.Element);
                xvr.AddKeys("shipping_cost", XmlNodeType.Element);
                xvr.AddKeys("category", XmlNodeType.Element);
                xvr.AddKeys("brand", XmlNodeType.Element);

                Product p = new Product();

                xvr.CreateReader(file, new XmlReaderSettings { CloseInput = true });
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["ean"][XmlNodeType.Element];
                    p.Title = dkd["title"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Element];
                    p.Price = dkd["price"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Url = dkd["deep_link"][XmlNodeType.Element];
                    p.Image_Loc = dkd["image_large"][XmlNodeType.Element];
                    p.Category = dkd["category"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["shipping_cost"][XmlNodeType.Element];
                    p.Affiliate = "Amazon";
                    p.FileName = file;
                    p.Webshop = fileUrl;
                    p.AffiliateProdID = p.Url != null ? p.Url.ToSHA256() : "";

                    if (p.Price == null || p.Currency == null) break;

                    if (p.Price.Contains(p.Currency)) p.Price = p.Price.Split(p.Currency.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    if (p.Price.Contains("£")) 
                        p.Price = p.Price.Split("£".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].Trim();

                    products.Add(p);
                    p = new Product();

                    if (products.Count >= PackageSize)
                    {
                        yield return products;
                        products.Clear();
                    }
                }

                yield return products;
            }
        }

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Directory not found: " + dir);
                yield break;
            }

            Console.WriteLine("Started reading from: " + dir);

            List<Product> products = new List<Product>();
            string[] filePaths = Util.ConcatArrays(Directory.GetFiles(dir, "*.xml"), Directory.GetFiles(dir, "*.csv"));

            // Initialize XmlValueReader and its keys
            XmlValueReader xvr = new XmlValueReader();
            xvr.ProductEnd = "product";
            xvr.AddKeys("ean", XmlNodeType.Element);
            xvr.AddKeys("productname", XmlNodeType.Element);
            xvr.AddKeys("brandname", XmlNodeType.Element);
            xvr.AddKeys("currentprice", XmlNodeType.Element);
            xvr.AddKeys("currency", XmlNodeType.Element);
            xvr.AddKeys("validuntil", XmlNodeType.Element);
            xvr.AddKeys("deeplinkurl", XmlNodeType.Element);
            xvr.AddKeys("imagesmallurl", XmlNodeType.Element);
            xvr.AddKeys("productcategory", XmlNodeType.Element);
            xvr.AddKeys("productdescriptionslong", XmlNodeType.Element);
            xvr.AddKeys("lastupdate", XmlNodeType.Element);
            xvr.AddKeys("shipping", XmlNodeType.Element);
            xvr.AddKeys("availabilty", XmlNodeType.Element);
            xvr.AddKeys("belboon_productnumber", XmlNodeType.Element);

            Product p = new Product();

            foreach (string file in filePaths)
            {
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["ean"][XmlNodeType.Element];
                    p.Title = dkd["productname"][XmlNodeType.Element];
                    p.Brand = dkd["brandname"][XmlNodeType.Element];
                    p.Price = dkd["currentprice"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.ValidUntil = dkd["validuntil"][XmlNodeType.Element];
                    p.Url = dkd["deeplinkurl"][XmlNodeType.Element];
                    p.Image_Loc = dkd["imagesmallurl"][XmlNodeType.Element];
                    p.Category = dkd["productcategory"][XmlNodeType.Element];
                    p.Description = dkd["productdescriptionslong"][XmlNodeType.Element];
                    p.LastModified = dkd["lastupdate"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["shipping"][XmlNodeType.Element];
                    p.Stock = dkd["availabilty"][XmlNodeType.Element];
                    p.AffiliateProdID = dkd["belboon_productnumber"][XmlNodeType.Element];
                    p.Affiliate = "Belboon";
                    p.FileName = file;
                    p.Webshop = fileUrl;
                    products.Add(p);
                    p = new Product();

                    if (products.Count > PackageSize)
                    {
                        yield return products;
                        products.Clear();
                    }
                }
                yield return products;
                products.Clear();
            }
        }
    }
}
