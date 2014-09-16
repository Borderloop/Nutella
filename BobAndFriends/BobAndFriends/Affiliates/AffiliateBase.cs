using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BobAndFriends
{
    public abstract class AffiliateBase
    {
        /// <summary>
        /// The name of the affiliate
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// This methoed will read all products from all datafiles in the given directory.
        /// </summary>
        /// <param name="dir">The directory</param>
        /// <returns>An enumerable list of lists of products.</returns>
        public abstract System.Collections.Generic.IEnumerable<List<Product>> ReadFromDir(string dir);
    }
}
