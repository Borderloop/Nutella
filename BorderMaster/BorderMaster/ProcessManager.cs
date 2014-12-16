using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace BorderMaster
{
    public class ProcessManager
    {
        public ILookup<int, string> ContinuousProcessPool { get; set; }
        public ILookup<int, string> AsyncProcessPool { get; set; }
        public DateTime StartTime { get; set; }


        public ProcessManager()
        { }

        public void RunContinuousProcessPool()
        {
            Queue<List<ProcessStartInfo>> processQueue = new Queue<List<ProcessStartInfo>>();
            var SortedContinuousProcessPool = from pair in ContinuousProcessPool orderby pair.Key ascending select pair;
            foreach(var group in SortedContinuousProcessPool)
            {
                List<ProcessStartInfo> list = new List<ProcessStartInfo>();
                foreach(string str in group)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(str);
                    startInfo.UseShellExecute = true;
                    list.Add(startInfo);
                }               
                processQueue.Enqueue(list);
            }

            List<ProcessStartInfo> currentProcessInfo = new List<ProcessStartInfo>();
            while((currentProcessInfo = processQueue.Dequeue()) != null)
            {
                string busyWith = "Busy with: " + Path.GetFileNameWithoutExtension(currentProcessInfo.First().FileName);
                foreach(ProcessStartInfo info in currentProcessInfo.Skip(1))
                {
                    busyWith += " and " + Path.GetFileNameWithoutExtension(info.FileName);
                }

                string nextUp = "Next up: " + Path.GetFileNameWithoutExtension(processQueue.Peek().First().FileName);
                foreach(ProcessStartInfo info in processQueue.Peek().Skip(1))
                {
                    nextUp += " and " + Path.GetFileNameWithoutExtension(info.FileName);
                }
                Console.Clear();
                Console.WriteLine("---------- BorderMaster console application ----------");
                Console.WriteLine("Started running at " + StartTime);
                Console.WriteLine(busyWith);
                Console.WriteLine(nextUp);
                Action[] actions = new Action[currentProcessInfo.Count];
                for (int i = 0; i < currentProcessInfo.Count; i++)
                {
                    int copy = i;
                    actions[copy] = new Action(() =>
                    {
                        Process process = Process.Start(currentProcessInfo[copy]);
                        process.WaitForExit();
                    });
                }

                Parallel.Invoke(actions);
                processQueue.Enqueue(currentProcessInfo);
            }
        }

        public void RunAsyncProcessPool()
        {
            /*
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
             */
        }
    }
}
