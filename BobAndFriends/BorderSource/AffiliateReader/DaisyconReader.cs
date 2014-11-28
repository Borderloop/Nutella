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

namespace BorderSource.AffiliateReader
{
    /// <summary>
    /// This class represents the reading from the .xml files delivered by WebgainsReader.
    /// The reading is automated by using the XmlValueReader.
    /// </summary>
    public class DaisyconReader : AffiliateReaderBase
    {

        public override string Name { get { return "Daisycon"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            

            //Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.ProductEnd = "item";
                xvr.AddKeys("ean_code", XmlNodeType.Element);
                xvr.AddKeys("title", XmlNodeType.Element);
                xvr.AddKeys("brand", XmlNodeType.Element);
                xvr.AddKeys("minimum_price", XmlNodeType.Element);
                xvr.AddKeys("link", XmlNodeType.Element);
                xvr.AddKeys("img_medium", XmlNodeType.Element);
                xvr.AddKeys("category", XmlNodeType.Element);
                xvr.AddKeys("description", XmlNodeType.Element);
                xvr.AddKeys("shipping_cost", XmlNodeType.Element);
                xvr.AddKeys("shippingcost", XmlNodeType.Element);
                xvr.AddKeys("stock", XmlNodeType.Element);
                xvr.AddKeys("shipping_duration_descr", XmlNodeType.Element);
                xvr.AddKeys("shipping_duration", XmlNodeType.Element);
                xvr.AddKeys("daisycon_unique_id", XmlNodeType.Element);

                Product p = new Product();
               
                xvr.CreateReader(file, new XmlReaderSettings { CloseInput = true });
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["ean_code"][XmlNodeType.Element];
                    p.Title = dkd["title"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Element];
                    p.Price = dkd["minimum_price"][XmlNodeType.Element];
                    p.Url = dkd["link"][XmlNodeType.Element];
                    p.Image_Loc = dkd["img_medium"][XmlNodeType.Element];
                    p.Category = dkd["category"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["shipping_cost"][XmlNodeType.Element] == default(string) ? dkd["shippingcost"][XmlNodeType.Element] : dkd["shipping_duration"][XmlNodeType.Element];
                    p.DeliveryTime = dkd["shipping_duration"][XmlNodeType.Element];
                    p.Stock = dkd["stock"][XmlNodeType.Element] == default(string) ? dkd["shipping_duration_descr"][XmlNodeType.Element] : dkd["stock"][XmlNodeType.Element];
                    p.AffiliateProdID = dkd["daisycon_unique_id"][XmlNodeType.Element];
                    p.Currency = "EUR";
                    p.Affiliate = "Daisycon";
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
            xvr.ProductEnd = "item";
            xvr.AddKeys("ean_code", XmlNodeType.Element);
            xvr.AddKeys("title", XmlNodeType.Element);
            xvr.AddKeys("brand", XmlNodeType.Element);
            xvr.AddKeys("minimum_price", XmlNodeType.Element);
            xvr.AddKeys("link", XmlNodeType.Element);
            xvr.AddKeys("img_medium", XmlNodeType.Element);
            xvr.AddKeys("category", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("shipping_cost", XmlNodeType.Element);
            xvr.AddKeys("shippingcost", XmlNodeType.Element);
            xvr.AddKeys("stock", XmlNodeType.Element);
            xvr.AddKeys("shipping_duration_descr", XmlNodeType.Element);
            xvr.AddKeys("shipping_duration", XmlNodeType.Element);
            xvr.AddKeys("daisycon_unique_id", XmlNodeType.Element);

            Product p = new Product();

            foreach (string file in filePaths)
            {
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                // If the webshop is not found in the webshop list no further processing needed.
                if (!Lookup.WebshopLookup.Contains(fileUrl))
                {
                   /*using (Logger logger = new Logger(Statics.LoggerPath, true))
                    {
                        logger.WriteLine("Webshop not found in database: " + fileUrl + " from " + Name);
                    }*/
                    continue;
                }
                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.EAN = dkd["ean_code"][XmlNodeType.Element];
                    p.Title = dkd["title"][XmlNodeType.Element];
                    p.Brand = dkd["brand"][XmlNodeType.Element];
                    p.Price = dkd["minimum_price"][XmlNodeType.Element];
                    p.Url = dkd["link"][XmlNodeType.Element];
                    p.Image_Loc = dkd["img_medium"][XmlNodeType.Element];
                    p.Category = dkd["category"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.DeliveryCost = dkd["shipping_cost"][XmlNodeType.Element] == default(string) ? dkd["shippingcost"][XmlNodeType.Element] : dkd["shipping_duration"][XmlNodeType.Element];
                    p.DeliveryTime = dkd["shipping_duration"][XmlNodeType.Element];
                    p.Stock = dkd["stock"][XmlNodeType.Element] == default(string) ? dkd["shipping_duration_descr"][XmlNodeType.Element] : dkd["stock"][XmlNodeType.Element];
                    p.AffiliateProdID = dkd["daisycon_unique_id"][XmlNodeType.Element];
                    p.Currency = "EUR";
                    p.Affiliate = "Daisycon";
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
