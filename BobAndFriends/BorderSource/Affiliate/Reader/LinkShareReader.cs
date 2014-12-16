using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.ProductAssociation;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace BorderSource.Affiliate.Reader
{
    public class LinkshareReader : AffiliateReaderBase
    {
        public override string Name
        {
            get { return "Linkshare"; }
        }

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tried to access LinkshareReader from ReadFromDir(). This is not implemented because it's obsolete.");
            yield break;
        }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
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
                        if (reader[0] == "TLR" || reader[0] == "HDR") continue;
                        Product p = new Product()
                        {
                            Affiliate = "Linkshare",
                            AffiliateProdID = reader[0],
                            Brand = reader[16],
                            Category = reader[3],
                            Currency = reader[25],
                            DeliveryCost = reader[17],
                            EAN = reader[23], 
                            FileName = file,
                            Image_Loc = reader[6],
                            Price = reader[13],
                            Stock = reader[22],
                            Title = reader[1],
                            Url = reader[4],
                            Webshop = fileUrl
                        };
                        products.Add(p);
                        if (products.Count >= PackageSize)
                        {
                            yield return products;
                            products.Clear();
                        }
                    }
                    yield return products;
                    products.Clear();
                }
            }
            yield break;
        }
    }
}
