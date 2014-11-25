using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class GeneralStatisticsMapper : IStatisticsMapper
    {
        private static GeneralStatisticsMapper _instance;
        public static GeneralStatisticsMapper Instance
        {
            get
            {
                if (_instance == null) _instance = new GeneralStatisticsMapper();
                return _instance;
            }
        }

        private Dictionary<string, IStatistics> _map = new Dictionary<string, IStatistics>();
        public Dictionary<string, IStatistics> map 
        { 
            get 
            {
                return _map;
            }
        }
        public void Increment(string name) 
        {
            if (!map.ContainsKey(name)) Add(name, new GeneralStatistics());
            ((GeneralStatistics)map[name]).count++;
        }

        public void Increment(string name, int amount)
        {
            if (!map.ContainsKey(name)) Add(name, new GeneralStatistics());
            ((GeneralStatistics)map[name]).count += amount;
        }

        public void Add(string key, IStatistics statics)
        {
            map.Add(key, statics);
        }

        public int Get(string key)
        {
            return map.ContainsKey(key) ? ((GeneralStatistics)map[key]).count : 0;
        }
    }
}
