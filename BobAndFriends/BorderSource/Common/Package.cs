using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class Package
    {
        public ICollection<Product> products;
        public string Webshop;

        public static Package CreateNew(ICollection<Product> products)
        {
            Package package = new Package();
            package.products = new List<Product>(products);
            package.Webshop = products.First().Webshop;
            return package;
        }
    }
}
