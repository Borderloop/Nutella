using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BorderSource.ProductAssociation;

namespace BorderSource.Affiliate.Reader
{
    public abstract class AffiliateReaderBase
    {
        public int PackageSize { get; set; }

        /// <summary>
        /// The name of the affiliate
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// This methoed will read all products from all datafiles in the given directory.
        /// </summary>
        /// <param name="dir">The directory</param>
        /// <returns>An enumerable list of lists of products.</returns>
        public abstract IEnumerable<List<Product>> ReadFromDir(string dir);

        public abstract IEnumerable<List<Product>> ReadFromFile(string file);
    }
}
