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
    
    public partial class vbobdata
    {
        public vbobdata()
        {
            this.vbob_suggested = new HashSet<vbob_suggested>();
        }
    
        public int id { get; set; }
        public string title { get; set; }
        public string ean { get; set; }
        public string sku { get; set; }
        public string brand { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public Nullable<bool> rerun { get; set; }
        public string image_loc { get; set; }
        public Nullable<short> country_id { get; set; }
    
        public virtual ICollection<vbob_suggested> vbob_suggested { get; set; }
        public virtual country country { get; set; }
    }
}
