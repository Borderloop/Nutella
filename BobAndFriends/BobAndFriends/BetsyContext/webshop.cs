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
    
    public partial class webshop
    {
        public webshop()
        {
            this.affiliate = new HashSet<affiliate>();
            this.language = new HashSet<language>();
            this.mark = new HashSet<mark>();
            this.payment_method = new HashSet<payment_method>();
            this.sender = new HashSet<sender>();
            this.country1 = new HashSet<country>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public short country_id { get; set; }
        public string logo_small { get; set; }
        public string logo_large { get; set; }
        public Nullable<decimal> shipping_cost { get; set; }
    
        public virtual ICollection<affiliate> affiliate { get; set; }
        public virtual country country { get; set; }
        public virtual ICollection<language> language { get; set; }
        public virtual ICollection<mark> mark { get; set; }
        public virtual ICollection<payment_method> payment_method { get; set; }
        public virtual ICollection<sender> sender { get; set; }
        public virtual ICollection<country> country1 { get; set; }
    }
}
