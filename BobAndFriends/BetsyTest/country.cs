//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BetsyTest
{
    using System;
    using System.Collections.Generic;
    
    public partial class country
    {
        public country()
        {
            this.biggest_price_differences = new HashSet<biggest_price_differences>();
            this.country_price_differences = new HashSet<country_price_differences>();
            this.title = new HashSet<title>();
            this.webshop = new HashSet<webshop>();
            this.webshop1 = new HashSet<webshop>();
        }
    
        public short id { get; set; }
        public string extension { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<biggest_price_differences> biggest_price_differences { get; set; }
        public virtual ICollection<country_price_differences> country_price_differences { get; set; }
        public virtual ICollection<title> title { get; set; }
        public virtual ICollection<webshop> webshop { get; set; }
        public virtual ICollection<webshop> webshop1 { get; set; }
    }
}