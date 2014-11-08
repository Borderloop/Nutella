using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Queue
{
    public interface IQueue<T> where T : class
    {
        Queue<T> Queue { get; }   
        void Enqueue(T element);
        T Dequeue();
    }
}
