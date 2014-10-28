using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BorderSource.Common
{
    public class TimeStatisticsMapper
    {
        public static Dictionary<string, TimeStatistics> map = new Dictionary<string, TimeStatistics>();

        public static void StartTimeMeasure(string name)
        {
            if (!map.Keys.Contains(name)) map.Add(name, new TimeStatistics());
            map[name].StartStopwatch();       
        }

        public static void StopTimeMeasure(string name)
        {
            map[name].StopStopwatch();    
        }
    }
}
