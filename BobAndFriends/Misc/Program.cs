using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using BorderSource.ProductAssociation;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Xml;

namespace Misc
{
    class Program
    {

        static void Main(string[] args)
        {          
            Action action2 = new Action(
                () =>
            {
                
                int i = 0;
                Console.WriteLine(i);
            });

            action2.Invoke();
            Console.Read();
        }
    }

}


