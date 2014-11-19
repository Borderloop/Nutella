using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class GeneralStatistics : IStatistics
    {
        public volatile int count;
        private string _Description = "General statistics";
        public string Description { get { return _Description; } }
    }
}
