using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BorderSource.Common
{
    public class TimeStatistics : IStatistics
    {
        public TimeSpan maxTime = new TimeSpan();        
        public TimeSpan minTime = new TimeSpan();
        public TimeSpan averageTime = new TimeSpan();

        private Stopwatch _sw = new Stopwatch();
        private int timeSpanAmount = 0;

        private string _Description = "Statistics for measuring the time that methods need to complete.";
        public string Description { get { return _Description; } }

        public void StartStopwatch()
        {
            _sw.Restart();
        }

        public void StopStopwatch()
        {
            _sw.Stop();
            AddTimeSpan(_sw.Elapsed);
        }

        public void AddTimeSpan(TimeSpan ts)
        {
            timeSpanAmount++;
            if (averageTime.Equals(TimeSpan.Zero)) 
                averageTime = ts;
            else
            {
                long totalTicks = averageTime.Ticks * (timeSpanAmount - 1) + ts.Ticks;
                averageTime = new TimeSpan((long)(totalTicks / (double)timeSpanAmount));
            }

            if (ts > maxTime) maxTime = ts;
            if (minTime.Equals(TimeSpan.Zero)) minTime = ts;
            if (ts < minTime) minTime = ts;
        }
    }
}
