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
    
    public partial class country
    {
        public country()
        {
            this.title = new HashSet<title>();
            this.webshop = new HashSet<webshop>();
            this.webshop1 = new HashSet<webshop>();
            this.vbobdata = new HashSet<vbobdata>();
        }
    
        public short id { get; set; }
        public string extension { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<title> title { get; set; }
        public virtual ICollection<webshop> webshop { get; set; }
        public virtual ICollection<webshop> webshop1 { get; set; }
        public virtual ICollection<vbobdata> vbobdata { get; set; }
    }
}
