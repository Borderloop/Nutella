using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            INIFile ini = new INIFile(@"C:/BorderSource/BorderMaster/settings/master.ini");
            ProcessManager manager = new ProcessManager();
            manager.ContinuousProcessPool = ini.GetAllValuesFromSection("Continuous");
            manager.AsyncProcessPool = ini.GetAllValuesFromSection("Async");
            manager.StartTime = DateTime.Now;
            Action[] actionList = new Action[] { 
                () => { manager.RunContinuousProcessPool(); },
                () => { manager.RunAsyncProcessPool(); }
            };
            Parallel.Invoke(actionList);
        }
    }
}
