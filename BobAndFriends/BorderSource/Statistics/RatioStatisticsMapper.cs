using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Statistics
{
    public class RatioStatisticsMapper : IStatisticsMapper
    {
        private static RatioStatisticsMapper _instance;
        public static RatioStatisticsMapper Instance
        {
            get
            {
                if (_instance == null) _instance = new RatioStatisticsMapper();
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

        public void Add(string key, bool firstIsTotal = false, params IStatistics[] statistics)
        {
            if (!map.ContainsKey(key)) map.Add(key, new RatioStatistics());
            ((RatioStatistics)map[key]).Statistics = statistics;
            ((RatioStatistics)map[key]).FirstIsTotal = firstIsTotal;
        }

        public void Add(string key, IStatistics statistics)
        {
            map.Add(key, statistics);
        }

        public float[] GetRatios(string key)
        {
            return map.ContainsKey(key) ? ((RatioStatistics)map[key]).CalculateRatios() : new float[0];
        }
    }
}
