
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
    
public partial class language
{

    public language()
    {

        this.webshop = new HashSet<webshop>();

    }


    public short id { get; set; }

    public string language1 { get; set; }

    public string short_language { get; set; }



    public virtual ICollection<webshop> webshop { get; set; }

}

}
