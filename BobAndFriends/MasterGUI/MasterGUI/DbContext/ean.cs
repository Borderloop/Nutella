// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated from a template.
// 
//      Manual changes to this fichier may cause unexpected behavior in your application.
//      Manual changes to this fichier will be overwritten if the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace MasterGUI.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class ean
    {
        public long ean1 { get; set; }
        public int article_id { get; set; }
    
        public virtual article article { get; set; }
    }
}
