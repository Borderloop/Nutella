// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated from a template.
// 
//      Manual changes to this file may cause unexpected behavior in your application.
//      Manual changes to this file will be overwritten if the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace BorderSource.BetsyContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class country_price_differences
    {
        public int id { get; set; }
        public int article_id { get; set; }
        public short country_id { get; set; }
        public decimal difference { get; set; }
        public System.DateTime last_updated { get; set; }
        public int product_id { get; set; }
        public decimal difference_percentage { get; set; }
    
        public virtual article article { get; set; }
        public virtual country country { get; set; }
        public virtual product product { get; set; }
    }
}
