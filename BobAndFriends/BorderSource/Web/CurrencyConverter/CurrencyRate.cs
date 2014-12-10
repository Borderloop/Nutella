using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Web.CurrencyConverter
{
    public class CurrencyRate
    {
        public string To { get; set; }
        public string From { get; set; }
        public double Rate { get; set; }
    }
}
