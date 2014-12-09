using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Statistics
{
    public class RatioStatistics : IStatistics
    {
        private string _Description = "Statistics for measuring the ratio between different statistics.";
        public string Description { get { return _Description; } }

        public ICollection<IStatistics> Statistics { get; set; }
        public bool FirstIsTotal { get; set; }

        /// <summary>
        /// Currently only has an implementation for GeneralStatistics. All other statistics will be ignored.
        /// </summary>
        /// <returns>An array containing ratios.</returns>
        public float[] CalculateRatios()
        {
            if (!Statistics.All(s => s.GetType() == typeof(GeneralStatistics))) return new float[0];
            else
            {
                float[] resultSet = new float[Statistics.Count];
                float total;
                if (FirstIsTotal)
                    total = ((GeneralStatistics)Statistics.First()).count;
                else
                    total = Statistics.Sum(s => (float)((GeneralStatistics)s).count);
                for(int i = 0; i < Statistics.Count; i++)
                {
                    resultSet[i] = ((GeneralStatistics)Statistics.ElementAt(i)).count / total;
                }
                return resultSet;
            }
        }
    }
}
