//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BorderSource.BetsyContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class articletemp
    {
        public articletemp()
        {
            this.eantemp = new HashSet<eantemp>();
            this.skutemp = new HashSet<skutemp>();
            this.titletemp = new HashSet<titletemp>();
            this.categorytemp = new HashSet<categorytemp>();
        }
    
        public int id { get; set; }
        public string brand { get; set; }
        public string description { get; set; }
        public string image_loc { get; set; }
    
        public virtual ICollection<eantemp> eantemp { get; set; }
        public virtual ICollection<skutemp> skutemp { get; set; }
        public virtual ICollection<titletemp> titletemp { get; set; }
        public virtual ICollection<categorytemp> categorytemp { get; set; }
    }
}
