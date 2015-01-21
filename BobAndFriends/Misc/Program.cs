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

            List<string> names = new List<string>() { "James", "John", "Freek" };
            var allNamesWithJ = names.Select(s => s.StartsWith("J"));



            Enumerable.Range(0, 100).ToList().ForEach(i => Console.WriteLine(i % 3 == 0 ? i % 8 == 0 ? "Borderloop" : "Border" : i % 8 == 0 ? "loop" : i.ToString()));
            Console.Read();
        }
    }
}


