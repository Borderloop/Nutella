using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BorderMaster
{
    public class ProcessManager
    {
        public Dictionary<int, string> ContinuousProcessPool { get; set; }
        public Dictionary<int, string> AsyncProcessPool { get; set; }
        public DateTime StartTime { get; set; }


        public ProcessManager()
        { }

        public void RunContinuousProcessPool()
        {
            Queue<ProcessStartInfo> processQueue = new Queue<ProcessStartInfo>();
            var SortedContinuousProcessPool = from pair in ContinuousProcessPool orderby pair.Key ascending select pair;
            foreach(KeyValuePair<int, string> process in SortedContinuousProcessPool)
            {              
                ProcessStartInfo startInfo = new ProcessStartInfo(process.Value);
                startInfo.UseShellExecute = true;
                processQueue.Enqueue(startInfo);
            }

            ProcessStartInfo currentProcessInfo = new ProcessStartInfo();
            Process currentProcess;
            while((currentProcessInfo = processQueue.Dequeue()) != null)
            {
                Console.Clear();
                Console.WriteLine("---------- BorderMaster console application ----------");
                Console.WriteLine("Started running at " + StartTime);
                Console.WriteLine("Busy with: " + currentProcessInfo.FileName);
                Console.WriteLine("Next up: " + processQueue.Peek().FileName);
                currentProcess = Process.Start(currentProcessInfo);
                currentProcess.WaitForExit();
                processQueue.Enqueue(currentProcessInfo);
            }
        }

        public void RunAsyncProcessPool()
        {
            Action[] actionList = new Action[AsyncProcessPool.Count];
            for(int i = 0; i < AsyncProcessPool.Count; i++)
            {
                if (!AsyncProcessPool.ContainsKey(i)) continue;
                string processPath = AsyncProcessPool[i];
                Action action = new Action(() =>
                {
                    ProcessStartInfo info = new ProcessStartInfo(processPath);
                    info.UseShellExecute = true;
                    Process process = Process.Start(info);
                    process.WaitForExit();
                });
                actionList[i] = action;
            }
            Parallel.Invoke(actionList);
        }
    }
}
