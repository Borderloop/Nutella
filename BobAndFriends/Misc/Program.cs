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
        static bool Done = false;
        static Thread producer = new Thread(new ThreadStart(FillQueue));
        static Thread consumer = new Thread(new ThreadStart(EmptyQueue));


        static void Main(string[] args)
        {
            producer.Start();
            consumer.Start();

            while(!Done)
            {
                Console.Clear();
                Console.WriteLine("Queue size: " + SingletonQueue<int?>.Instance.Queue.Count);
                Thread.Sleep(20);
                GC.Collect();
            }
            Console.WriteLine("Done");
            Console.Read();
        }

        static void FillQueue()
        {
            int count = 0;
            while (count < 100000)
            {
                count++;
                SingletonQueue<int?>.Instance.Enqueue(new Random().Next());
                Thread.Sleep(10);
            }
        }

        static void EmptyQueue()
        {
            int? output;
            while ((output = SingletonQueue<int?>.Instance.Dequeue()) != null)
            {
                Thread.Sleep(20);
            }
            Done = true;
        }
    }
}
