using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class PropertyStatisticsMapper
    {
        public static Dictionary<string, PropertyStatistics> map = new Dictionary<string, PropertyStatistics>();

        public static void Add(string name, string input)
        {
            if (!map.Keys.Contains(name)) map.Add(name, new PropertyStatistics());
            map[name].Add(input);
        }
    }
}
