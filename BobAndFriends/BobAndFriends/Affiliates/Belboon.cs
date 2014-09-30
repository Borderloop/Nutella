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
    public class Belboon : AffiliateBase
    {
        public override string Name { get { return "Belboon"; } }

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
            xvr.AddKeys("belboonproductnumber", XmlNodeType.Element);

            Product p = new Product();

            foreach (string file in filePaths)
            {
                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())                               
                {
                    p.EAN = Regex.IsMatch(dkd["ean"][XmlNodeType.Element].EmptyNull(), @"^[0-9]{10,13}$") ? dkd["ean"][XmlNodeType.Element] : ""; 
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
                    p.AfiiliateProdID = dkd["belboonproductnumber"][XmlNodeType.Element];
                    p.Affiliate = "Belboon";
                    p.FileName = file;
                    p.Webshop = "www." + Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
                    products.Add(p);
                    p = new Product();
                }
            }
            yield return products;
            products.Clear();
        }
    }
}

