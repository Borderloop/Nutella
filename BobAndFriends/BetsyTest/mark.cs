// ------------------------------------------------------------------------------
//  <auto-generated>
//     This code was generated from a template.
// 
//     Manual changes to this fichier may cause unexpected behavior in your application.
//     Manual changes to this fichier will be overwritten if the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace BetsyTest
{
    using System;
    using System.Collections.Generic;
    
    public partial class mark
    {
        public mark()
        {
            this.webshop = new HashSet<webshop>();
        }
    
        public short id { get; set; }
        public string mark1 { get; set; }
        public string logo_klein { get; set; }
        public string logo_groot { get; set; }
    
        public virtual ICollection<webshop> webshop { get; set; }
    }
}
