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
    /// <summary>
    /// This class represents the reading from the .xml files delivered by WebgainsReader.
    /// The reading is automated by using the XmlValueReader.
    /// </summary>
    public class WebgainsReader : AffiliateReaderBase
    {

        public override string Name { get { return "Webgains"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            

            // Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
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

                xvr.CreateReader(file, new XmlReaderSettings { CloseInput = true });
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    // Fill the product with fields
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
                    p.AffiliateProdID = dkd["product_id"][XmlNodeType.Element] + dkd["program_id"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Affiliate = "Webgains";
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

            // Initialize XmlValueReader and its keys
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
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    // Fill the product with fields
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
                    p.AffiliateProdID = dkd["product_id"][XmlNodeType.Element] + dkd["program_id"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Affiliate = "Webgains";
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
    
