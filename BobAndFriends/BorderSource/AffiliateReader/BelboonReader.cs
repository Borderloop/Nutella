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
    public class BelboonReader : AffiliateReaderBase
    {

        public override string Name { get { return "Belboon"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {            

            //Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

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

                xvr.CreateReader(file, new XmlReaderSettings { CloseInput = true });
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

            //Initialize XmlValueReader and its keys
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

