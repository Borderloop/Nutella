using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProductFeedReader
{
    public abstract class AffiliateBase
    {
        /// <summary>
        /// The name of the affiliate
        /// </summary>
        public abstract string Name { get; }
        public abstract System.Collections.Generic.IEnumerable<List<Product>> ReadFromDir(string dir);
    }
}
