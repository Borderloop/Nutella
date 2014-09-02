using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeTrackerReader
{
    public class Product
    {
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

    }
}
