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
    
    public partial class vbob_suggested
    {
        public int id { get; set; }
        public int article_id { get; set; }
        public int vbob_id { get; set; }
    
        public virtual article article { get; set; }
        public virtual vbobdata vbobdata { get; set; }
    }
}
