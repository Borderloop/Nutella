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
    public class LDLCReader : AffiliateReaderBase
    {
        public override string Name
        {
            get { return "LDLC"; }
        }

        private string LDLCID = "5478285febd4c";

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tried to access LDLCReader from ReadFromDir(). This is not implemented because it's obsolete.");
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
                        Product p = new Product()
                        {
                            Affiliate = "LDLC",
                            AffiliateProdID = reader[0],
                            Brand = reader[1],
                            Category = reader[3],
                            Currency = "EUR",
                            DeliveryCost = reader[11],
                            EAN = reader[5],
                            FileName = file,
                            Image_Loc = reader[13],
                            Price = reader[8],
                            Stock = reader[14],
                            Title = reader[7],
                            Url = reader[12],
                            Webshop = fileUrl
                        };

                        p.Url.Replace("[identifiant-Affilie]", LDLCID);
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
