using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.IO;

namespace Bumper
{
    class Program
    {
        static void Main(string[] args)
        {

            INIFile ini = new INIFile(@"C:\BorderSoftware\BobAndFriends\settings\baf.ini");
            Dictionary<string, string> properties = ini.GetAllValues();

            string cmd = String.Format("-h{0} -u{1} -p{2} --result-file={3} --port={4} {5}", properties["db_source"], properties["db_userid"], properties["db_password"], properties["dump_path"] + "fullDump_" + DateTime.Now.ToString("ddMMyyyy") + ".sql",properties["db_port"], properties["db_name"]);

            ProcessStartInfo info = new ProcessStartInfo(properties["mysqldumpexe_path"], cmd);
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            using (Process p = new Process())
            {
                p.StartInfo = info;
                p.Start();
                p.WaitForExit();
            }

            if (Directory.GetFiles(properties["dump_path"]).Count() > int.Parse(properties["max_dump_files"]))
            {
                foreach (var file in new DirectoryInfo(properties["dump_path"]).GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(int.Parse(properties["max_dump_files"])))
                {
                    file.Delete();
                }
            }
        }
    }
}