using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using BorderSource.Statistics;
using BorderSource.Common;

namespace BorderSource.ProductAssociation
{
    public class ProductFilter
    {
        public bool LogProperties { get; set; }

        public ProductFilter()
        {
            LogProperties = false;
        }
     
        public bool CheckProperties(Product p)
        {
            foreach (var prop in p.GetType().GetProperties())
            {
                //We can safely assume all fields are strings, but for the future we throw an exception if a field isn't a string or null.
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
                        if ((parsedPrice = decimal.Parse(prop.GetValue(p) as string)) == 0)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            return false;
                        }
                        break;

                    case "DeliveryCost":
                        prop.SetValue(p, (prop.GetValue(p) as string).Replace(',', '.'));
                        prop.SetValue(p, Regex.IsMatch(prop.GetValue(p) as string, @"^\d+(.\d{1,2})?$") ? prop.GetValue(p) : null);
                        break;

                    case "Title":
                        if ((prop.GetValue(p) as string).Length > Statics.maxTitleSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
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
                        if ((prop.GetValue(p) as string).Length > Statics.maxBrandSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
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
                        if ((prop.GetValue(p) as string).Length > Statics.maxSkuSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).ToUpper().Trim());
                        }
                        break;

                    case "Image_Loc":
                        if ((prop.GetValue(p) as string).Length > Statics.maxImageUrlSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Category":
                        if ((prop.GetValue(p) as string).Length > Statics.maxCategorySize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "DeliveryTime":
                        if ((prop.GetValue(p) as string).Length > Statics.maxShipTimeSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Webshop":
                        if ((prop.GetValue(p) as string) == "www.hardware.nl") p.GetType().GetProperty("SKU").SetValue(p, "");
                        if ((prop.GetValue(p) as string).Length > Statics.maxWebShopUrlSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Url":
                        if ((prop.GetValue(p) as string).Length > Statics.maxDirectLinkSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "Affiliate":
                        if ((prop.GetValue(p) as string).Length > Statics.maxAffiliateNameSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
                            return false;
                        }
                        else
                        {
                            prop.SetValue(p, (prop.GetValue(p) as string).Trim());
                        }
                        break;

                    case "AffiliateProdID":
                        if ((prop.GetValue(p) as string).Length > Statics.maxAffiliateProductIdSize)
                        {
                            if (LogProperties) PropertyStatisticsMapper.Instance.Add(prop.Name, prop.GetValue(p) as string);
                            prop.SetValue(p, "--!--" + prop.GetValue(p) + "--!--");
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
            return (!string.IsNullOrEmpty(p.EAN)
                || !string.IsNullOrEmpty(p.SKU))
                && !string.IsNullOrEmpty(p.AffiliateProdID)
                && !string.IsNullOrEmpty(p.Category)
                && !string.IsNullOrEmpty(p.Title)
                && !string.IsNullOrEmpty(p.Url);
        }
    }
}
