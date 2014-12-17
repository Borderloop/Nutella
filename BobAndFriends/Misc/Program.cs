using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using BorderSource.ProductAssociation;
using LumenWorks.Framework.IO.Csv;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Misc
{
    class Program
    {

        static void Main(string[] args)
        {
            var DateOfBirth2 = DateTime.Parse(string.Format("{0}-{1}-{2}", "1924", "10", "19"));
            Console.WriteLine(DateOfBirth2);
            var DateOfBirth = DateTime.Parse(string.Format("{0}-{1}-{2}", "19", "10", "1924"), CultureInfo.InvariantCulture);
            Console.WriteLine(DateOfBirth);
            Console.Read();
        }
    }

}


