using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Statistics
{
    public interface IStatisticsMapper
    {
        Dictionary<string, IStatistics> map { get; }

        void Add(string key, IStatistics statistics);
    }
}
