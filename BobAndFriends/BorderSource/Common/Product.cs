using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;

namespace BorderSource.Common
{
    /// <summary>
    /// Default product class for storing data.
    /// </summary>
    public class Product
    {
        
        //All the data will be read as a String. Therefore, we store them in String objects.
        private string _Title = "";
        public string Title { get { return _Title; } set { _Title = value; } }

        private string _Url = "";
        public string Url { get { return _Url; } set { _Url = value; } }

        private string _Image_Loc = "";
        public string Image_Loc { get { return _Image_Loc; } set { _Image_Loc = value; } }

        private string _Description = "";
        public string Description { get { return _Description; } set { _Description = value; } }

        private string _Category = "";
        public string Category { get { return _Category; } set { _Category = value; } }

        private string _Price = "";
        public string Price { get { return _Price; } set { _Price = value; } }

        private string _Currency = "";
        public string Currency { get { return _Currency; } set { _Currency = value; } }

        private string _DeliveryCost = "";
        public string DeliveryCost { get { return _DeliveryCost; } set { _DeliveryCost = value; } }

        private string _DeliveryTime = "";
        public string DeliveryTime { get { return _DeliveryTime; } set { _DeliveryTime = value; } }

        private string _EAN = "";
        public string EAN { get { return _EAN; } set { _EAN = value; } }

        private string _Stock = "";
        public string Stock { get { return _Stock; } set { _Stock = value; } }

        private string _Brand = "";
        public string Brand { get { return _Brand; } set { _Brand = value; } }

        private string _LastModified = "";
        public string LastModified { get { return _LastModified; } set { _LastModified = value; } }

        private string _ValidUntil = "";
        public string ValidUntil { get { return _ValidUntil; } set { _ValidUntil = value; } }

        private string _SKU = "";
        public string SKU { get { return _SKU; } set { _SKU = value; } }

        //These properties do not need private values since they are always set.
        public string Affiliate { get; set; }
        public string AffiliateProdID { get; set; }
        public string FileName { get; set; }
        public string Webshop { get; set; }

        public bool CleanupFields()
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                //We can safely assume all fields are strings, but for the future we throw an exception if a field isn't a string or null.
                object type = prop.GetValue(this);
                decimal parsedPrice;
                if (!(type is string) && type != null) throw new NotImplementedException();
                //Make sure the fields are DEFINITELY not null
                if (prop.GetValue(this) == null)
                    prop.SetValue(this, "");

                switch(prop.Name)
                {
                    case "EAN":
                        prop.SetValue(this, Regex.IsMatch(prop.GetValue(this) as string, @"^[0-9]{10,13}$") ? prop.GetValue(this) : "");
                        break;

                    case "Price":
                        prop.SetValue(this, (prop.GetValue(this) as string).Replace(',', '.'));
                        prop.SetValue(this, Regex.IsMatch(prop.GetValue(this) as string, @"^\d+(.\d{1,2})?$") ? prop.GetValue(this) : "");
                        if (prop.GetValue(this) as string == "") return false;
                        if ((parsedPrice = decimal.Parse(prop.GetValue(this) as string))==0) return false; 
                        break;

                    case "DeliveryCost":
                        prop.SetValue(this, (prop.GetValue(this) as string).Replace(',', '.'));
                        prop.SetValue(this, Regex.IsMatch(prop.GetValue(this) as string, @"^\d+(.\d{1,2})?$") ? prop.GetValue(this) : null);
                        break;

                    case "Title":
                        if ((prop.GetValue(this) as string).Length > Statics.maxTitleSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "Brand":
                        if ((prop.GetValue(this) as string).Length > Statics.maxBrandSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "SKU":
                        if ((prop.GetValue(this) as string).Length > Statics.maxSkuSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "Image_Loc":
                        if ((prop.GetValue(this) as string).Length > Statics.maxImageUrlSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "Category":
                        if ((prop.GetValue(this) as string).Length > Statics.maxCategorySize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "DeliveryTime":
                        if ((prop.GetValue(this) as string).Length > Statics.maxShipTimeSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "Webshop":
                        if ((prop.GetValue(this) as string).Length > Statics.maxWebShopUrlSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "Url":
                        if ((prop.GetValue(this) as string).Length > Statics.maxDirectLinkSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "Affiliate":
                        if ((prop.GetValue(this) as string).Length > Statics.maxAffiliateNameSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;

                    case "AffiliateProdID":
                        if ((prop.GetValue(this) as string).Length > Statics.maxAffiliateProductIdSize)
                        {
                            prop.SetValue(this, "--!--" + prop.GetValue(this) + "--!--");
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var prop in this.GetType().GetProperties())
            {
                sb.AppendLine(prop.Name + ": " + prop.GetValue(this));
            }
            return sb.ToString();
        }
    }
}
