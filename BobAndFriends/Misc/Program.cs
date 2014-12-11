using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using BorderSource.ProductAssociation;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Xml;

namespace Misc
{
    class Program
    {

        static void Main(string[] args)
        {
            foreach (string file in Directory.GetFiles(@"C:\BorderSoftware\Product Feeds\AffiliateWindow"))
                ReadFromFile(file);
        }

        public static void ReadFromFile(string file)
        {
            string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
            using (var sr = new StreamReader(file))
            {
                sr.ReadLine();
                using (CsvReader reader = new CsvReader(sr, false, '|'))
                {
                    List<Product> products = new List<Product>();
                    while (reader.ReadNextRecord())
                    {

                        Product p = new Product()
                        {
                            Affiliate = "AffiliateWindow",
                            AffiliateProdID = reader[0],
                            Brand = reader[15],
                            Category = reader[2],
                            Currency = reader[12],
                            DeliveryCost = reader[11],
                            EAN = reader[20],
                            FileName = file,
                            Image_Loc = reader[4],
                            Price = reader[13],
                            Stock = reader[19],
                            Title = reader[7],
                            Url = reader[3],
                            Webshop = fileUrl
                        };

                        p.Price = p.Price.Trim(p.Currency.ToCharArray());
                        products.Add(p);
                        if (products.Count >= 10)
                        {
                            products.Clear();
                        }
                    }
                    products.Clear();
                }
            }
        }
    }
}


