//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BobAndFriends
{
    using System;
    using System.Collections.Generic;
    
    public partial class product
    {
        public int id { get; set; }
        public int article_id { get; set; }
        public string ship_time { get; set; }
        public Nullable<decimal> ship_cost { get; set; }
        public decimal price { get; set; }
        public string webshop_url { get; set; }
        public string direct_link { get; set; }
        public System.DateTime last_modified { get; set; }
        public System.DateTime valid_until { get; set; }
        public string affiliate_name { get; set; }
        public string affiliate_unique_id { get; set; }
    
        public virtual article article { get; set; }
    }
}
