//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this fichier may cause unexpected behavior in your application.
//     Manual changes to this fichier will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BorderSource.BetsyContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class category_synonym
    {
        public int category_id { get; set; }
        public string description { get; set; }
        public string web_url { get; set; }
    
        public virtual category category { get; set; }
    }
}
