using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.ProductAssociation;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using BorderSource.Loggers;
using BorderSource.Statistics;

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
                    reader.MissingFieldAction = MissingFieldAction.ParseError;
                    reader.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                    reader.ParseError += ParseError;
                    reader.SkipEmptyLines = true;
                    List<Product> products = new List<Product>();
                    while (reader.ReadNextRecord())
                    {
                        if (reader.FieldCount < 20) continue;
                        try
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
                GeneralStatisticsMapper.Instance.Increment("AffiliateWindow: MISSING FIELD ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else if (e.Error is MalformedCsvException)
            {
                GeneralStatisticsMapper.Instance.Increment("AffiliateWindow: MALFORMED CSV ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else
            {
                GeneralStatisticsMapper.Instance.Increment("AffiliateWindow: PARSE ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
        }
    }
}
