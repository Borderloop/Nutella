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
            RunSomeTasks();
            Console.Read();
        }

        public static void RunSomeTasks()
        {
            var tasks = new Task[50];

            for (int i = 0; i < tasks.Length; i++)
            {
                int copy = i;
                tasks[copy] = new Task(() => StartSomeMethod(copy));
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks.Count(t => t.Status == TaskStatus.Running) >= 5)
                {
                    Task.WaitAny(tasks);
                }
                tasks[i].Start();
            }
        }

        public static void StartSomeMethod(int id)
        {
            Console.WriteLine("Started {0}", id);
            int i = 0;
            while (i < Int32.MaxValue) i++;
            Console.WriteLine("Ended {0}", id);
        }
    }
}
