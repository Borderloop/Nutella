using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class DualKeyDictionary<K1, K2, T> : Dictionary<K1, Dictionary<K2, T>>
    {
        public T this[K1 key1, K2 key2]
        {
            get
            {
                if(!ContainsKey(key1))                   
                    throw new ArgumentException("Dictionary does not contain key " + key1.ToString());
                if (!this[key1].ContainsKey(key2))
                    throw new ArgumentException("Dictionary does not contain key " + key2.ToString());
                return base[key1][key2];
            }
            set
            {
                if(!ContainsKey(key1))
                    this[key1] = new Dictionary<K2, T>();
               this[key1][key2] = value;
            }
        }

        public void Add(K1 key1, K2 key2, T value)
        {
            if(!ContainsKey(key1))
                this[key1] = new Dictionary<K2, T>();
            this[key1][key2] = value;
        }

        public bool ContainsKey(K1 key1, K2 key2)
        {
            return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
        }

        public void ClearValues()
        {
            foreach(K1 key1 in this.Keys.ToList())
            {
                foreach(K2 key2 in this[key1].Keys.ToList())
                {                    
                    this[key1][key2] = default(T);
                }
            }
        }
    }
}
