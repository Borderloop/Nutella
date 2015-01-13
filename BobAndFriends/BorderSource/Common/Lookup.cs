using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class Lookup
    {
        public static IDictionary<string, Webshop> WebshopLookup;
        public static ILookup<string, Category> CategoryLookup;
        public static ILookup<string, CategorySynonym> CategorySynonymLookup;
    }

    public class Category
    {
        public int Id;
        public string Description;
    }

    public class CategorySynonym
    {
        public string WebshopUrl;
        public int CategoryId;
        public string Description;

    }

    public struct Webshop
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public int CountryId { get; set; }
    }
}


