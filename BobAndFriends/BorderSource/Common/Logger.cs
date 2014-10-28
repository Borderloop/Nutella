using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace BorderSource.Common
{
    public class Logger : StreamWriter
    {

        public Logger(string logPath) : base(logPath) { }

        public void WriteStatistics()
        {
            base.WriteLine("Started writing statistics...");
            base.WriteLine();
            base.WriteLine("----------------------- TIME MEASURE STATISTICS -----------------------");
            foreach (KeyValuePair<string, TimeStatistics> pair in BorderSource.Common.TimeStatisticsMapper.map)
            {
                base.WriteLine("Stats for \"" + pair.Key + "\":");
                base.WriteLine("Average time: " + pair.Value.averageTime);
                base.WriteLine("Max time: " + pair.Value.maxTime);
                base.WriteLine("Min time: " + pair.Value.minTime);
                base.WriteLine();
            }
            base.WriteLine();
            base.WriteLine("----------------------- PROPERTY STATISTICS -----------------------");
            base.WriteLine();
            foreach (KeyValuePair<string, PropertyStatistics> pair in BorderSource.Common.PropertyStatisticsMapper.map)
            {
                base.WriteLine("Stats for \"" + pair.Key + "\":");
                base.WriteLine("Amount of roducts failed on this property: " + pair.Value.wrongPropertyCount);
                base.WriteLine("Average length: " + pair.Value.averagePropertyLength);
                base.WriteLine("Max length: " + pair.Value.maxPropertyLength);
                base.WriteLine("Distribution of lengths by range;");
                int sum;
                for (int i = 0; i <= 10; i++)
                {
                    sum = pair.Value.occurences.GetCumulativeValuesFromRange(i * 10, ((i + 1) * 10) - 1);
                    if(sum == 0) continue;
                    base.WriteLine("Range " + i * 10 + " - " + (((i + 1) * 10) - 1) + ": " + sum);
                }
                for (int i = 10; i < 30; i += 5)
                {
                    sum = pair.Value.occurences.GetCumulativeValuesFromRange(i * 10, ((i + 5) * 10) - 1);
                    if (sum == 0) continue;
                    base.WriteLine("Range " + i * 10 + " - " + (((i + 5) * 10) - 1) + ": " + sum);
                }
                base.WriteLine();
            }
            base.WriteLine("----------------------- END OF STATISTICS -----------------------");
        }
    }
}
