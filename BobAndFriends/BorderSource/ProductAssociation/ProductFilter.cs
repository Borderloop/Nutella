using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using BorderSource.Statistics;
using BorderSource.Common;
using BorderSource.Loggers;
using System.Globalization;

namespace BorderSource.ProductAssociation
{
    public class ProductFilter
    {
        public bool LogProperties { get; set; }
        public Dictionary<string, int> Maximums { get; set; }

        public IDictionary<string, decimal> TaxExclusiveWebshops { get; set; }

        public IDictionary<string, decimal> CurrencyRates { get; set; }

        public ProductFilter()
        {
            LogProperties = false;
        }
     
        public bool CheckProperties(Product p)
        {
            if (p == null) return false;
            foreach (var prop in p.GetType().GetProperties())
            {
                object type = prop.GetValue(p);
                decimal parsedPrice;
                if (!(type is string) && type != null) continue;
                // Make sure the fields are DEFINITELY not null
                if (prop.GetValue(p) == null)
                    prop.SetValue(p, "");

                switch (prop.Name)
                {
                    case "EAN":
                        string rawEan = prop.GetValue(p) as string;
                        long tempEan;
                        if (!long.TryParse(rawEan, out tempEan)) return false;
                        prop.SetValue(p, Regex.IsMatch(tempEan.ToString(), @"^[0-9]{10,13}$") ? tempEan.ToString() : "");
                        if ((prop.GetValue(p) as string).Contains("00000000000") || (prop.GetValue(p) as string).Contains("999999999999"))
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        break;

                    case "Price":
                        if (p.Webshop.Contains("amazon"))
                        {
                            string[] parts;
                            if (p.Webshop.Contains("co.uk")) parts = (prop.GetValue(p) as string).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            else parts = (prop.GetValue(p) as string).Swap(',', '.').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            prop.SetValue(p, String.Join("", parts));
                        }
                        else prop.SetValue(p, (prop.GetValue(p) as string).Replace(',', '.'));
                        prop.SetValue(p, Regex.IsMatch(prop.GetValue(p) as string, @"^\d+(.\d{1,2})?$") ? prop.GetValue(p) : "");
                        if ((prop.GetValue(p) as string) == "")
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        if ((parsedPrice = decimal.Parse(prop.GetValue(p) as string, NumberStyles.Any, CultureInfo.InvariantCulture)) == 0)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        decimal rate = CurrencyRates == null ? 1 : CurrencyRates.ContainsKey(p.Currency.ToUpper()) ? CurrencyRates[p.Currency.ToUpper()] : 1;
                        prop.SetValue(p, (parsedPrice / rate).ToString().Replace(',', '.'));
                        break;

                    case "DeliveryCost":
                        prop.SetValue(p, (prop.GetValue(p) as string).Replace(',','.'));
                        prop.SetValue(p, Regex.IsMatch(prop.GetValue(p) as string, @"^\d+(.\d{1,2})?$") ? prop.GetValue(p) : null);
                        break;

                    case "Title":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_title_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).EscapeChars().Trim());
                            if (prop.GetValue(p) as string == "")
                            {
                                if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                                return false;
                            }
                        }
                        break;

                    case "Brand":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_brand_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "SKU":
                        if ((prop.GetValue(p) as string).Contains("!"))
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        if ((prop.GetValue(p) as string).Length > Maximums["max_sku_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).ToUpper().Trim());
                        }
                        break;

                    case "Image_Loc":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_imageurl_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Category":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_category_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "DeliveryTime":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_shiptime_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Webshop":
                        if ((prop.GetValue(p) as string) == "www.hardware.nl") p.GetType().GetProperty("SKU").SetValue(p, "");
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_webshopurl_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Url":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_directlink_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Affiliate":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_affiliatename_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "AffiliateProdID":
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_affiliateproductid_size"])
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).ToLower().Trim());
                        }
                        break;
                    case "Description":
                        prop.SetValue(p, "");
                        break;

                    case "AdditionalEANs":
                        List<string> EANs = prop.GetValue(p) as List<string>;
                        if (EANs == null || EANs.Count == 0) break;
                        List<string> CorrectEANs = EANs.Where(e => Regex.IsMatch(e, @"^[0-9]{10,13}$")).ToList();
                        prop.SetValue(p, CorrectEANs);
                        break;
                    default: break;
                }
            }

            if (TaxExclusiveWebshops.ContainsKey(p.Webshop))
            {
                decimal TaxInclusivePrice = Math.Round(decimal.Parse(p.Price, NumberStyles.Any, CultureInfo.InvariantCulture) * TaxExclusiveWebshops[p.Webshop], 2);
                p.Price = TaxInclusivePrice.ToString().Replace(',','.');
            }

            return (!string.IsNullOrWhiteSpace(p.EAN)
                || !string.IsNullOrWhiteSpace(p.SKU))
                && !string.IsNullOrWhiteSpace(p.AffiliateProdID)
                && !string.IsNullOrWhiteSpace(p.Title)
                && !string.IsNullOrWhiteSpace(p.Url);
        }
    }
}
