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
            string s = "1,231,293.093".Swap(',', '.');

            //Enumerable.Range(0, 100).ToList().ForEach(i => Console.WriteLine(i % 3 == 0 ? i % 8 == 0 ? "Borderloop" : "Border" : i % 8 == 0 ? "loop" : i.ToString()));
            Console.Read();
        }

      
    }

    static class Util
    {
        public static string Swap(this string input, char firstChar, char secondChar)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in input)
            {
                if (c == firstChar) builder.Append(secondChar);
                else if (c == secondChar) builder.Append(firstChar);
                else builder.Append(c);
            }
            return builder.ToString();
        }
    }
}


