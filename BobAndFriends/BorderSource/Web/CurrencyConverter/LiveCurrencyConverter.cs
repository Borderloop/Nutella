using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml;

namespace BorderSource.Web.CurrencyConverter
{
    public static class LiveCurrencyConverter
    {
        public static decimal Convert(string from, string to, decimal amount)
        {
            string url = String.Format("https://www.google.com/finance/converter?a={2}&from={0}&to={1}", from, to, amount);
            WebClient client = new WebClient();
            string rates = client.DownloadString(url);
            CurrencyRate rate = new JavaScriptSerializer().Deserialize<CurrencyRate>(rates); //needs refactoring
            return amount * (decimal)rate.Rate;
        }

        public static Dictionary<string, decimal> GetCurrencyRatesToEUR()
        {
            Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
            using(XmlReader reader = XmlReader.Create(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"))
            {
                reader.ReadToFollowing("Cube");
                while(reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.GetAttribute("currency") != null)
                        dic.Add(reader.GetAttribute("currency"), decimal.Parse(reader.GetAttribute("rate"), System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            dic.Add("EUR", 1);
            return dic;
        }
    }
}
