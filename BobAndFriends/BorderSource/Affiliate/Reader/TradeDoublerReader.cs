﻿using System;
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
    public class TradeDoublerReader : AffiliateReaderBase
    {

        public override string Name { get { return "TradeDoubler"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {         
            //  Initialize XmlValueReader and its keys
            using (XmlValueReader xvr = new XmlValueReader())
            {
                List<Product> products = new List<Product>();
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
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

                xvr.CreateReader(file, new XmlReaderSettings { CloseInput = true });
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    //  Fill the product with fields
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
                string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');

                xvr.CreateReader(file);
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in xvr.ReadProducts())
                {
                    // Fill the product with fields
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
    
