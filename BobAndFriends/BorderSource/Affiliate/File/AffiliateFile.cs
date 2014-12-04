using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Affiliate.Reader;
using System.IO;

namespace BorderSource.Affiliate.File
{
    public class AffiliateFile
    {
        public string Name { get; set; }

        private string _FileLocation;
        public string FileLocation
        {
            get
            {
                return _FileLocation;
            }
            set
            {
                if (Path.GetExtension(value).ToLower() == ".txt") Path.ChangeExtension(value, ".csv");
                _FileLocation = value;
            }
        }
    }
}
