using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using BorderSource.Statistics;
using BorderSource.Common;
using System.Globalization;

namespace BorderSource.ProductAssociation
{
    public class ProductFilter
    {
        public bool LogProperties { get; set; }
        public Dictionary<string, int> Maximums { get; set; }

        public HashSet<string> TaxExclusiveWebshops { get; set; }

        public ProductFilter()
        {
            LogProperties = false;
        }
     
        public bool CheckProperties(Product p)
        {
            foreach (var prop in p.GetType().GetProperties())
            {
                object type = prop.GetValue(p);
                decimal parsedPrice;
                if (!(type is string) && type != null) continue;
                //Make sure the fields are DEFINITELY not null
                if (prop.GetValue(p) == null)
                    prop.SetValue(p, "");

                switch (prop.Name)
                {
                    case "EAN":
                        prop.SetValue(p, Regex.IsMatch(prop.GetValue(p) as string, @"^[0-9]{10,13}$") ? (prop.GetValue(p) as string).Trim() : "");
                        if ((prop.GetValue(p) as string).Contains("00000000000"))
                        {
                            if(LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        break;

                    case "Price":
                        prop.SetValue(p, (prop.GetValue(p) as string).Replace(',', '.'));
                        prop.SetValue(p, Regex.IsMatch(prop.GetValue(p) as string, @"^\d+(.\d{1,2})?$") ? prop.GetValue(p) : "");
                        if (prop.GetValue(p) as string == "")
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        if ((parsedPrice = decimal.Parse(prop.GetValue(p) as string, NumberStyles.Any, CultureInfo.InvariantCulture)) == 0)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
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
                        if ((prop.GetValue(p) as string).Length >= Maximums["max_directlink_size"] - 10)
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
                    default: break;
                }
            }

            if (TaxExclusiveWebshops.Contains(p.Webshop))
            {
                decimal TaxInclusivePrice = Math.Round(decimal.Parse(p.Price, NumberStyles.Any, CultureInfo.InvariantCulture) * (decimal)1.21, 2);
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
