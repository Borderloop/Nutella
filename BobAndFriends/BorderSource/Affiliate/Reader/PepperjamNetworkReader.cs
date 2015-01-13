using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.ProductAssociation;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using BorderSource.Loggers;
using BorderSource.Common;

namespace BorderSource.Affiliate.Reader
{
    public class PepperjamNetworkReader : AffiliateReaderBase
    {
        public override string Name
        {
            get { return "PepperjamNetwork"; }
        }


        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tried to access PepperjamReader from ReadFromDir(). This is not implemented because it's obsolete.");
            yield break;
        }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
            using (var sr = new StreamReader(file))
            {
                sr.ReadLine();
                using (CsvReader reader = new CsvReader(sr, false, ','))
                {
                    reader.MissingFieldAction = MissingFieldAction.ParseError;
                    reader.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                    reader.ParseError += ParseError;
                    reader.SkipEmptyLines = true;
                    List<Product> products = new List<Product>();
                    while (reader.ReadNextRecord())
                    {
                        try
                        {
                            Product p = new Product()
                            {
                                Affiliate = "PepperjamNetwork",
                                Brand = reader[32],
                                Category = reader[73],
                                Currency = reader[2],
                                EAN = reader[66],
                                FileName = file,
                                Price = reader[79],
                                Stock = reader[76],
                                Title = reader[40],
                                SKU = reader[62],
                                Image_Loc = reader[25],
                                Description = reader[12],
                                Url = reader[9],
                                Webshop = fileUrl
                            };

                            p.AffiliateProdID = p.Url.ToSHA256();
                            products.Add(p);
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.WriteLine("BAD CSV FILE: " + fileUrl + " ERROR: " + e.Message);
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
                Logger.Instance.WriteLine("Pepperjam: MISSING FIELD ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else if (e.Error is MalformedCsvException)
            {
                Logger.Instance.WriteLine("Pepperjam: MALFORMED CSV ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else
            {
                Logger.Instance.WriteLine("Pepperjam: PARSE ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
        }

    }
}

