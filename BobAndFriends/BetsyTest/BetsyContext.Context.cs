﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BetsyTest
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BetsyEntities : DbContext
    {   
        public virtual DbSet<affiliate> affiliate { get; set; }
        public virtual DbSet<article> article { get; set; }
        public virtual DbSet<biggest_price_differences> biggest_price_differences { get; set; }
        public virtual DbSet<category> category { get; set; }
        public virtual DbSet<category_synonym> category_synonym { get; set; }
        public virtual DbSet<country> country { get; set; }
        public virtual DbSet<country_price_differences> country_price_differences { get; set; }
        public virtual DbSet<ean> ean { get; set; }
        public virtual DbSet<language> language { get; set; }
        public virtual DbSet<mark> mark { get; set; }
        public virtual DbSet<payment_method> payment_method { get; set; }
        public virtual DbSet<product> product { get; set; }
        public virtual DbSet<residue> residue { get; set; }
        public virtual DbSet<sender> sender { get; set; }
        public virtual DbSet<sku> sku { get; set; }
        public virtual DbSet<title> title { get; set; }
        public virtual DbSet<title_synonym> title_synonym { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<vbob_suggested> vbob_suggested { get; set; }
        public virtual DbSet<vbobdata> vbobdata { get; set; }
        public virtual DbSet<webshop> webshop { get; set; }
    }
}
