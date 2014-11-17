﻿using System;
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

        public Logger(string logPath, bool append) : base(logPath, append) { }

        public void WriteStatistics()
        {
            base.WriteLine("Started writing statistics...");
            base.WriteLine();
            base.WriteLine("----------------------- GENERAL STATISTICS -----------------------");
            foreach(KeyValuePair<string, IStatistics> pair in BorderSource.Common.GeneralStatisticsMapper.Instance.map)
            {
                base.WriteLine(pair.Key + ": " + ((GeneralStatistics)pair.Value).count);
            }
            base.WriteLine();
            base.WriteLine("----------------------- TIME MEASURE STATISTICS -----------------------");
            base.WriteLine();
            foreach (KeyValuePair<string, IStatistics> pair in BorderSource.Common.TimeStatisticsMapper.Instance.map)
            {
                base.WriteLine("Stats for \"" + pair.Key + "\":");
                base.WriteLine("Average time: " + ((TimeStatistics)pair.Value).averageTime);
                base.WriteLine("Max time: " + ((TimeStatistics)pair.Value).maxTime);
                base.WriteLine("Min time: " + ((TimeStatistics)pair.Value).minTime);
                base.WriteLine();
            }
            base.WriteLine();
            base.WriteLine("----------------------- PROPERTY STATISTICS -----------------------");
            base.WriteLine();
            foreach (KeyValuePair<string, IStatistics> pair in BorderSource.Common.PropertyStatisticsMapper.Instance.map)
            {
                base.WriteLine("Stats for \"" + pair.Key + "\":");
                base.WriteLine("Amount of roducts failed on this property: " + ((PropertyStatistics)pair.Value).wrongPropertyCount);
                base.WriteLine("Average length: " + ((PropertyStatistics)pair.Value).averagePropertyLength);
                base.WriteLine("Max length: " + ((PropertyStatistics)pair.Value).maxPropertyLength);
                base.WriteLine("Distribution of lengths by range;");
                int sum;
                for (int i = 0; i <= 10; i++)
                {
                    sum = ((PropertyStatistics)pair.Value).occurences.GetCumulativeValuesFromRange(i * 10, ((i + 1) * 10) - 1);
                    if(sum == 0) continue;
                    base.WriteLine("Range " + i * 10 + " - " + (((i + 1) * 10) - 1) + ": " + sum);
                }
                for (int i = 10; i < 30; i += 5)
                {
                    sum = ((PropertyStatistics)pair.Value).occurences.GetCumulativeValuesFromRange(i * 10, ((i + 5) * 10) - 1);
                    if (sum == 0) continue;
                    base.WriteLine("Range " + i * 10 + " - " + (((i + 5) * 10) - 1) + ": " + sum);
                }
                base.WriteLine();
            }
            base.WriteLine("----------------------- END OF STATISTICS -----------------------");
        }
    }
}
