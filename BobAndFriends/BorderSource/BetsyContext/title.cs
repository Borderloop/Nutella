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
    
    public partial class title
    {
        public title()
        {
            this.title_synonym = new HashSet<title_synonym>();
        }
    
        public int id { get; set; }
        public string title1 { get; set; }
        public short country_id { get; set; }
        public int article_id { get; set; }
    
        public virtual article article { get; set; }
        public virtual country country { get; set; }
        public virtual ICollection<title_synonym> title_synonym { get; set; }
    }
}
