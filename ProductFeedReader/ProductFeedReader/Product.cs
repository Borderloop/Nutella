using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ProductFeedReader
{
    /// <summary>
    /// Default product class for storing data.
    /// </summary>
    public class Product
    {
        //All the data will be read as a String. Therefore, we store them in String objects.
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } 
        public string Price { get; set; } 
        public string Currency { get; set; }
        public string DeliveryCost { get; set; }
        public string DeliveryTime { get; set; }
        public string EAN { get; set; }
        public string Stock { get; set; }
        public string Brand { get; set; }
        public string LastModified { get; set; }
        public string ValidUntil { get; set; }     
        public string Affiliate { get; set; }
        public string FileName { get; set; }
    }
}
