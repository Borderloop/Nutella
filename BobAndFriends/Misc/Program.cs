using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;

namespace Misc
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = ">";
            Console.WriteLine(s);
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(s);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            Console.WriteLine(asciiStr);
            Console.Read();
        }     
    }
}
