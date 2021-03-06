﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BorderSource.ProductAssociation
{
    /// <summary>
    /// Default product class for storing data.
    /// </summary>
    public class Product : IEquatable<Product>
    {
        
        // All the data will be read as a String. Therefore, we store them in String objects.
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

        private string _Currency = "";
        public string Currency { get { return _Currency; } set { _Currency = value; } }

        private string _Price = "";
        public string Price { get { return _Price; } set { _Price = value; } }

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

        private bool _IsManuallyAdded = false;
        public bool IsManuallyAdded { get { return _IsManuallyAdded; } set { _IsManuallyAdded = value; } }

        // These properties do not need private values since they are always set.
        public string Affiliate { get; set; }
        public string AffiliateProdID { get; set; }
        public string FileName { get; set; }
        public string Webshop { get; set; }

        public bool IsValidated { get; set; }

        private List<string> _AdditionalEANs = new List<string>();
        public List<string> AdditionalEANs { get { return _AdditionalEANs; } set { _AdditionalEANs = value; } }

        public bool HasMultipleEANs { get { return _AdditionalEANs.Count > 0; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var prop in this.GetType().GetProperties())
            {
                sb.AppendLine(prop.Name + ": " + prop.GetValue(this));
            }
            return sb.ToString();
        }

        public bool Equals(Product other)
        {
            if (other == null) return false;
            return this.Affiliate == other.Affiliate && this.AffiliateProdID == other.AffiliateProdID;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Product productObj = obj as Product;
            if (productObj == null) return false;
            return Equals(productObj);
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode() ^ EAN.GetHashCode();
        }
    }
}
