using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvilBatcher
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
                Dictionary<string, string> settings = new BorderSource.Common.INIFile("C://BorderSoftware//BobAndFriends//EvilBatcher//settings//eb.ini").GetAllValues();
                Database.Instance.Connect(settings["dbsource"], settings["dbname"], settings["dbuid"], settings["dbpw"]);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EvilBatcher());
            } catch(Exception e)
            {
                MessageBox.Show("Error: " + e.ToString());
            } finally
            {
                Database.Instance.CloseLogger();
            }

        }
    }
}
