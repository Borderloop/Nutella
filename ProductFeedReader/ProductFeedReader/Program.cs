using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProductFeedReader
{
    class Program
    {
        /// <summary>
        /// Main method will only start the ProductFeedReader
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ProductFeedReader pfr = new ProductFeedReader();
            pfr.Start();
        }

    }
}
