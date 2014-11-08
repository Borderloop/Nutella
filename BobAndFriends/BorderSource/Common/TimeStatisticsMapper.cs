using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BorderSource.Common
{
    public class TimeStatisticsMapper : IStatisticsMapper
    {
        private static TimeStatisticsMapper _instance;
        public static TimeStatisticsMapper Instance
        {
            get
            {
                if (_instance == null) _instance = new TimeStatisticsMapper();
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

        public void StartTimeMeasure(string key)
        {
            if (!map.Keys.Contains(key)) Add(key, new TimeStatistics());
            ((TimeStatistics)map[key]).StartStopwatch();       
        }

        public void StopTimeMeasure(string key)
        {
           ((TimeStatistics)map[key]).StopStopwatch();    
        }

        public void StopAll()
        {
            foreach(string key in map.Keys)
            {
                ((TimeStatistics)map[key]).StopStopwatch();
            }
        }

        public void Add(string key, IStatistics statistics)
        {
            map.Add(key, statistics);
        }
    }
}
