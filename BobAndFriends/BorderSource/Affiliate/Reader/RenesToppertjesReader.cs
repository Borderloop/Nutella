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
    public class RenesToppertjesReader : AffiliateReaderBase
    {
        public override string Name { get { return "Rene\'s Toppertjes"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {

            // Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.ProductEnd = "Product";
                xvr.AddKeys("EAN", XmlNodeType.Element);
                xvr.AddKeys("SKU", XmlNodeType.Element);
                xvr.AddKeys("Title", XmlNodeType.Element);
                xvr.AddKeys("Brand", XmlNodeType.Element);
                xvr.AddKeys("Price", XmlNodeType.Element);
                xvr.AddKeys("Deeplink", XmlNodeType.Element);
                xvr.AddKeys("Image", XmlNodeType.Element);
                xvr.AddKeys("Category", XmlNodeType.Element);
                xvr.AddKeys("Availabilty", XmlNodeType.Element);
                xvr.AddKeys("ProductID", XmlNodeType.Element);
                xvr.AddKeys("Webshop", XmlNodeType.Element);
                xvr.AddKeys("Shipcost", XmlNodeType.Element);

                Product p = new Product();

                xvr.CreateReader(file, new XmlReaderSettings { CloseInput = true });
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["EAN"][XmlNodeType.Element];
                    p.SKU = dkd["SKU"][XmlNodeType.Element];
                    p.Title = dkd["Title"][XmlNodeType.Element];
                    p.Brand = dkd["Brand"][XmlNodeType.Element];
                    p.Price = dkd["Price"][XmlNodeType.Element];
                    p.Url = dkd["Deeplink"][XmlNodeType.Element];
                    p.Image_Loc = dkd["Image"][XmlNodeType.Element];
                    p.Category = dkd["Category"][XmlNodeType.Element];
                    p.Stock = dkd["Availabilty"][XmlNodeType.Element];
                    p.AffiliateProdID = dkd["ProductID"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["Shipcost"][XmlNodeType.Element];
                    p.Affiliate = "None";
                    p.FileName = file;
                    p.Webshop = dkd["Webshop"][XmlNodeType.Element];
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
            xvr.ProductEnd = "Product";
            xvr.AddKeys("EAN", XmlNodeType.Element);
            xvr.AddKeys("SKU", XmlNodeType.Element);
            xvr.AddKeys("Title", XmlNodeType.Element);
            xvr.AddKeys("Brand", XmlNodeType.Element);
            xvr.AddKeys("Price", XmlNodeType.Element);
            xvr.AddKeys("Deeplink", XmlNodeType.Element);
            xvr.AddKeys("Image", XmlNodeType.Element);
            xvr.AddKeys("Category", XmlNodeType.Element);
            xvr.AddKeys("Availabilty", XmlNodeType.Element);
            xvr.AddKeys("ProductID", XmlNodeType.Element);

            Product p = new Product();

            foreach (string file in filePaths)
            {
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["EAN"][XmlNodeType.Element];
                    p.SKU = dkd["SKU"][XmlNodeType.Element];
                    p.Title = dkd["Title"][XmlNodeType.Element];
                    p.Brand = dkd["Brand"][XmlNodeType.Element];
                    p.Price = dkd["Price"][XmlNodeType.Element];
                    p.Url = dkd["Deeplink"][XmlNodeType.Element];
                    p.Image_Loc = dkd["Image"][XmlNodeType.Element];
                    p.Category = dkd["Category"][XmlNodeType.Element];
                    p.Stock = dkd["Availabilty"][XmlNodeType.Element];
                    p.AffiliateProdID = dkd["ProductID"][XmlNodeType.Element];
                    p.Affiliate = "None";
                    p.FileName = file;
                    p.Webshop = fileUrl;
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
    }
}
