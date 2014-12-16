using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class ConcurrentHashSet<T>
    {
        private readonly object Lock = new object();

        private HashSet<T> hash;

        public ConcurrentHashSet()
        {
            hash = new HashSet<T>();
        }

        public bool Contains(T key, IEqualityComparer<T> comparer)
        {
            lock(Lock)
            {
                return hash.Contains(key, comparer);
            }
        }

        public bool Contains(T key)
        {
            lock(Lock)
            {
                return hash.Contains(key);
            }
        }

        public void Add(T t)
        {
            lock(Lock)
            {
                hash.Add(t);
            }
        }

        public void Remove(T t)
        {
            lock(Lock)
            {
                hash.Remove(t);
            }
        }
    }
}
