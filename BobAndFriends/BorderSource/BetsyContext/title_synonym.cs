// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated from a template.
// 
//      Manual changes to this fichier may cause unexpected behavior in your application.
//      Manual changes to this fichier will be overwritten if the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace BorderSource.BetsyContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class title_synonym
    {
        public string title { get; set; }
        public int title_id { get; set; }
        public Nullable<short> occurrences { get; set; }
    
        public virtual title title1 { get; set; }
    }
}
