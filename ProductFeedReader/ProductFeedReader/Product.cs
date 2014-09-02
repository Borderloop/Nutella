using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TradeTrackerReader
{
    /// <summary>
    /// Default product class for storing data.
    /// </summary>
    public class Product
    {
        //All the data will be read as a String. Therefore, we store them in String objects.

        public string ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SKU { get; set; }
        public string DeliveryCost { get; set; }
        public string DeliveryTime { get; set; }
        public string EAN { get; set; }
        public string Stock { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Language { get; set; }
        public string Affiliate { get; set; }
        public string FileName { get; set; }

        /// <summary>
        /// Debug method for checking product data.
        /// </summary>
        /// <returns>A string shortly representing some product data</returns>
        public string toString()
        {
            return "Name: " + Name + ", EAN: " + EAN;
        }

    }
}
