using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.ProductAssociation;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using BorderSource.Loggers;
using System.Globalization;

namespace BorderSource.Affiliate.Reader
{
    public class JacobElektronikReader : AffiliateReaderBase
    {
        public override string Name
        {
            get { return "JacobElektronik"; }
        }

        private string BaseUrl = "http://www.jacob-computer.de/_artnr_XXXXXX.html?ref=1986";

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tried to access JacobElektronikReader from ReadFromDir(). This is not implemented because it's obsolete.");
            yield break;
        }

        public override IEnumerable<List<Product>> ReadFromFile(string file)
        {
            string fileUrl = Path.GetFileNameWithoutExtension(file).Split(null)[0].Replace('$', '/');
            using (var sr = new StreamReader(file))
            {
                sr.ReadLine();
                using (CsvReader reader = new CsvReader(sr, false, '\t'))
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
                                Affiliate = "JacobElektronik",
                                AffiliateProdID = reader[0],
                                Brand = reader[4],
                                Category = reader[2],
                                Currency = "EUR",
                                EAN = reader[3],
                                FileName = file,
                                Price = reader[7],
                                Stock = reader[5],
                                Title = reader[8],
                                Webshop = "www.jacob-computer.de"
                            };

                            float mass = 0;
                            if (float.TryParse(reader[13], NumberStyles.Any, CultureInfo.InvariantCulture, out mass)) p.DeliveryCost = GetDeliveryCostForMass(mass);
                            p.Url = BaseUrl.Replace("XXXXXX", p.AffiliateProdID);
                            products.Add(p);
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.WriteLine("BAD CSV FILE: www.www.jacob-computer.de ERROR: " + e.Message);
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
                Logger.Instance.WriteLine("JacobElektronik: MISSING FIELD ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else if (e.Error is MalformedCsvException)
            {
                Logger.Instance.WriteLine("JacobElektronik: MALFORMED CSV ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
            else
            {
                Logger.Instance.WriteLine("JacobElektronik: PARSE ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }
        }

        private string GetDeliveryCostForMass(float mass)
        {
            if (mass <= 6) return "9.99";
            else if (mass <= 12) return "20.99";
            else if (mass <= 20) return "24.99";
            else if (mass <= 30) return "32.99";
            else return "";
        }
    }
}
