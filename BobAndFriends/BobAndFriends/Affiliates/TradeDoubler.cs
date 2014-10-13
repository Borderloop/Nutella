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
    /// This class represents the reading from the .xml files delivered by Webgains.
    /// The reading is automated by using the XmlValueReader.
    /// </summary>
    public class TradeDoubler : AffiliateBase
    {
        // Stores the name of the website that is being processed.
        private string _fileUrl;

        public override string Name { get { return "TradeDoubler"; } }

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
            xvr.AddKeys("ean", XmlNodeType.Element);
            xvr.AddKeys("sku", XmlNodeType.Element);
            xvr.AddKeys("name", XmlNodeType.Element);
            xvr.AddKeys("brand", XmlNodeType.Element);
            xvr.AddKeys("price", XmlNodeType.Element);
            xvr.AddKeys("currency", XmlNodeType.Element);
            xvr.AddKeys("productUrl", XmlNodeType.Element);
            xvr.AddKeys("imageUrl", XmlNodeType.Element);
            xvr.AddKeys("TDCategoryName", XmlNodeType.Element);
            xvr.AddKeys("description", XmlNodeType.Element);
            xvr.AddKeys("shippingCost", XmlNodeType.Element);
            xvr.AddKeys("inStock", XmlNodeType.Element);
            xvr.AddKeys("deliveryTime", XmlNodeType.Element);
            xvr.AddKeys("TDProductId", XmlNodeType.Element);

            Product p = new Product();

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
                    xvr.CreateReader(file);
                    foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                    {
                        //Fill the product with fields
                        p.EAN = dkd["ean"][XmlNodeType.Element];
                        p.SKU = dkd["sku"][XmlNodeType.Element];
                        p.Title = dkd["name"][XmlNodeType.Element];
                        p.Brand = dkd["brand"][XmlNodeType.Element];
                        p.Price = dkd["price"][XmlNodeType.Element];
                        p.Url = dkd["productUrl"][XmlNodeType.Element];
                        p.Image_Loc = dkd["imageUrl"][XmlNodeType.Element];
                        p.Category = dkd["TDCategoryName"][XmlNodeType.Element];
                        p.Description = dkd["description"][XmlNodeType.Element];
                        p.DeliveryCost = dkd["shippingCost"][XmlNodeType.Element];
                        p.DeliveryTime = dkd["deliveryTime"][XmlNodeType.Element];
                        p.Stock = dkd["inStock"][XmlNodeType.Element];
                        p.AffiliateProdID = dkd["TDProductId"][XmlNodeType.Element];
                        p.Currency = dkd["currency"][XmlNodeType.Element];
                        p.Affiliate = "TradeDoubler";
                        p.FileName = file;
                        p.Webshop = _fileUrl;
                        products.Add(p);
                        p = new Product();
                    }
                }
            }
            yield return products;
            products.Clear();
        }
    }
}
    
