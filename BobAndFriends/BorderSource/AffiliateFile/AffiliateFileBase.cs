using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.AffiliateReader;

namespace BorderSource.AffiliateFile
{
    public abstract class AffiliateFileBase
    {
        public string Name;
        public string FileLocation;

        public abstract AffiliateReaderBase GetReader();
    }
}
