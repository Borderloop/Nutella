using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.ProductAssociation;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using BorderSource.Loggers;

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
                    reader.MissingFieldAction = MissingFieldAction.ParseError;
                    reader.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                    reader.ParseError += ParseError;
                    reader.SkipEmptyLines = true;
                    List<Product> products = new List<Product>();
                    while (reader.ReadNextRecord())
                    {
                        if (reader[0] == "TLR" || reader[0] == "HDR") continue;
                        try
                        {
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
                                Url = reader[5],
                                Webshop = fileUrl
                            };
                            products.Add(p);
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.WriteLine("BAD CSV FILE: " + fileUrl + " ERROR " + e.Message);
                        }
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

        private void ParseError(object sender, ParseErrorEventArgs e)
        {
            // if the error is that a field is missing, then skip to next line
            if (e.Error is MissingFieldCsvException)
            {
                Logger.Instance.WriteLine("Linkshare: MISSING FIELD ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else if (e.Error is MalformedCsvException)
            {
                Logger.Instance.WriteLine("Linkshare: MALFORMED CSV ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else
            {
                Logger.Instance.WriteLine("Linkshare: PARSE ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
        }

    }
}
