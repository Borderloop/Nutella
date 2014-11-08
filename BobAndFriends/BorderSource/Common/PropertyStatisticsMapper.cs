using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class PropertyStatisticsMapper : IStatisticsMapper
    {
        private static PropertyStatisticsMapper _instance;
        public static PropertyStatisticsMapper Instance
        {
            get
            {
                if (_instance == null) _instance = new PropertyStatisticsMapper();
                return _instance;
            }
        }


        private Dictionary<string, IStatistics> _map = new Dictionary<string,IStatistics>();
        public Dictionary<string, IStatistics> map
        {
            get
            {
                return _map;
            }
        }

        public void Add(string name, string input)
        {
            if (!map.Keys.Contains(name)) Add(name, new PropertyStatistics());
            ((PropertyStatistics)map[name]).Add(input);
        }

        public void Add(string key, IStatistics statistics)
        {
            map.Add(key, statistics);
        }
    }
}
