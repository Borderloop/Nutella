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
    
    public partial class sku
    {
        public string sku1 { get; set; }
        public int article_id { get; set; }
    
        public virtual article article { get; set; }
    }
}
