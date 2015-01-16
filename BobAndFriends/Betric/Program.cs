using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Betric
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Betric());
            }
            catch(Exception e)
            {
                using(StreamWriter writer = new StreamWriter(@"C:/log.txt"))
                {
                    writer.Write(e.Message);
                    writer.Flush();
                }

            }
        }
    }
}
