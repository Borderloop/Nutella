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
    public class CommissionJunctionReader : AffiliateReaderBase
    {

        public override string Name { get { return "CommissionJunction"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {            

            //Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.ProductEnd = "product";
                xvr.AddKeys("sku", XmlNodeType.Element);
                xvr.AddKeys("advertisercategory", XmlNodeType.Element);
                xvr.AddKeys("name", XmlNodeType.Element);
                xvr.AddKeys("manufacturer", XmlNodeType.Element);
                xvr.AddKeys("saleprice", XmlNodeType.Element);
                xvr.AddKeys("currency", XmlNodeType.Element);
                xvr.AddKeys("buyurl", XmlNodeType.Element);
                xvr.AddKeys("imageurl", XmlNodeType.Element);
                xvr.AddKeys("description", XmlNodeType.Element);
                xvr.AddKeys("lastupdated", XmlNodeType.Element);
                xvr.AddKeys("instock", XmlNodeType.Element);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;
                settings.CloseInput = true;
                Product p = new Product();                

                xvr.CreateReader(file, settings);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.SKU = dkd["sku"][XmlNodeType.Element];
                    p.Title = dkd["name"][XmlNodeType.Element];
                    p.Brand = dkd["manufacturer"][XmlNodeType.Element];
                    p.Price = dkd["saleprice"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Category = dkd["advertisercategory"][XmlNodeType.Element];
                    p.Url = dkd["buyurl"][XmlNodeType.Element];
                    p.Image_Loc = dkd["imageurl"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.LastModified = dkd["lastupdated"][XmlNodeType.Element];
                    p.Stock = dkd["instock"][XmlNodeType.Element];
                    p.Affiliate = "CommissionJunction";
                    p.FileName = file;
                    p.Webshop = fileUrl;

                    //Hash the title and the webshop into a unique ID, because CommissionJunctionReader didn't provide any
                    p.AffiliateProdID = (p.Title + p.Webshop).ToSHA256();

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

            //Initialize XmlValueReader and its keys
            XmlValueReader xvr = new XmlValueReader();
            xvr.ProductEnd = "product";
            xvr.AddKeys("sku", XmlNodeType.Element);
            xvr.AddKeys("advertisercategory", XmlNodeType.Element);
            xvr.AddKeys("name", XmlNodeType.Element);
            xvr.AddKeys("manufacturer", XmlNodeType.Element);
            xvr.AddKeys("saleprice", XmlNodeType.Element);
            xvr.AddKeys("currency", XmlNodeType.Element);
            xvr.AddKeys("buyurl", XmlNodeType.Element);
            xvr.AddKeys("imageurl", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("lastupdated", XmlNodeType.Element);
            xvr.AddKeys("instock", XmlNodeType.Element);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            Product p = new Product();

            foreach (string file in filePaths)
            {
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                // If the webshop is not found in the webshop list no further processing needed.
                if (!Lookup.WebshopLookup.Contains(fileUrl))
                {
                    /*using (Logger logger = new Logger(Statics.LoggerPath, true))
                    {
                        logger.WriteLine("Webshop not found in database: " + fichierUrl + " from " + Name);
                    }*/
                    continue;
                }
                xvr.CreateReader(file, settings);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    p.SKU = dkd["sku"][XmlNodeType.Element];
                    p.Title = dkd["name"][XmlNodeType.Element];
                    p.Brand = dkd["manufacturer"][XmlNodeType.Element];
                    p.Price = dkd["saleprice"][XmlNodeType.Element];
                    p.Currency = dkd["currency"][XmlNodeType.Element];
                    p.Category = dkd["advertisercategory"][XmlNodeType.Element];
                    p.Url = dkd["buyurl"][XmlNodeType.Element];
                    p.Image_Loc = dkd["imageurl"][XmlNodeType.Element];
                    p.Description = dkd["description"][XmlNodeType.Element];
                    p.LastModified = dkd["lastupdated"][XmlNodeType.Element];
                    p.Stock = dkd["instock"][XmlNodeType.Element];
                    p.Affiliate = "CommissionJunction";
                    p.FileName = file;
                    p.Webshop = fileUrl;

                    //Hash the title and the webshop into a unique ID, because CommissionJunctionReader didn't provide any
                    p.AffiliateProdID = (p.Title + p.Webshop).ToSHA256();

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

