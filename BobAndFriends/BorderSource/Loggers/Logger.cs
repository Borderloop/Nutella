using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using BorderSource.Statistics;
using BorderSource.Common;

namespace BorderSource.Loggers
{
    public class Logger : StreamWriter
    {
        /*
        private readonly object Lock = new object();

        public string LogPath { get; set; }

        private Logger _Instance;

        public Logger Instance
        {
            get
            {
                if(_Instance == null)
                {
                    lock(Lock)
                    {
                        if(_Instance == null)
                        {
                            _Instance = new Logger(LogPath, true);
                        }
                    }
                }
                return _Instance;
            }
        }
       
        public override void Write(string value)
        {
            lock (Lock)
            {
                base.Write(value);
            }
        }
        */
        public Logger(string logPath) : base(logPath) { }

        public Logger(string logPath, bool append) : base(logPath, append) { }

        public void WriteStatistics()
        {
            RatioStatisticsMapper.Instance.Add("Ratio between read and filtered products", GeneralStatisticsMapper.Instance.map["Products read"], GeneralStatisticsMapper.Instance.map["Wrong products"]);
            RatioStatisticsMapper.Instance.Add("Ratio between validated and saved products", GeneralStatisticsMapper.Instance.map["Total amount of products processed"], GeneralStatisticsMapper.Instance.map["Products saved"]);
            RatioStatisticsMapper.Instance.Add("Ratio between read and saved products", GeneralStatisticsMapper.Instance.map["Products read"], GeneralStatisticsMapper.Instance.map["Products saved"]);
            RatioStatisticsMapper.Instance.Add("Ratios between saved products", GeneralStatisticsMapper.Instance.map["Products saved"], GeneralStatisticsMapper.Instance.map["EAN matches"], GeneralStatisticsMapper.Instance.map["SKU matches"], GeneralStatisticsMapper.Instance.map["Existing products"]);
            RatioStatisticsMapper.Instance.Add("Ratios between saved products and total", GeneralStatisticsMapper.Instance.map["Products read"], GeneralStatisticsMapper.Instance.map["EAN matches"], GeneralStatisticsMapper.Instance.map["SKU matches"], GeneralStatisticsMapper.Instance.map["Existing products"]);
            
            base.WriteLine("Started writing statistics...");
            base.WriteLine();
            base.WriteLine("----------------------- GENERAL STATISTICS -----------------------");
            foreach(KeyValuePair<string, IStatistics> pair in GeneralStatisticsMapper.Instance.map)
            {
                base.WriteLine(pair.Key + ": " + ((GeneralStatistics)pair.Value).count);
            }
            base.WriteLine();
            base.WriteLine("----------------------- RATIO STATISTICS -----------------------");
            foreach (KeyValuePair<string, IStatistics> pair in RatioStatisticsMapper.Instance.map)
            {
                base.WriteLine(pair.Key + ":");
                float[] ratios = ((RatioStatistics)pair.Value).CalculateRatios();
                int i = 0;
                foreach(IStatistics statistics in (((RatioStatistics)pair.Value).Statistics))
                {
                    base.WriteLine(((GeneralStatistics)statistics).Name + ": " + Math.Round(ratios[i] * (100), 2) + "%");
                }
            }
            base.WriteLine();
            base.WriteLine("----------------------- TIME MEASURE STATISTICS -----------------------");
            base.WriteLine();
            foreach (KeyValuePair<string, IStatistics> pair in TimeStatisticsMapper.Instance.map)
            {
                base.WriteLine("Stats for \"" + pair.Key + "\":");
                base.Write("Average time: " + ((TimeStatistics)pair.Value).averageTime + "\t");
                base.Write("Max time: " + ((TimeStatistics)pair.Value).maxTime + "\t");
                base.Write("Min time: " + ((TimeStatistics)pair.Value).minTime + "\n");
                base.WriteLine();
            }
            base.WriteLine();
            base.WriteLine("----------------------- PROPERTY STATISTICS -----------------------");
            base.WriteLine();
            foreach (KeyValuePair<string, IStatistics> pair in PropertyStatisticsMapper.Instance.map)
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
