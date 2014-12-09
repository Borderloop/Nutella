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
    class AffiliateWindowReader : AffiliateReaderBase
    {
        public override string Name
        {
            get { return "AffiliateWindow"; }
        }

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tried to access BolReader from ReadFromDir(). This is not implemented because it's obsolete.");
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
                    while(reader.ReadNextRecord())
                    {
                        yield break;
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

                        p.Price.Trim(p.Currency.ToCharArray());
                        products.Add(p);
                        if(products.Count >= PackageSize)
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
