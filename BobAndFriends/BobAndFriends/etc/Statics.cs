using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobAndFriends
{
    public class Statics
    {
        /// <summary>
        /// The amount of ticks the productfeedreader has made.
        /// </summary>
        public static int TickCount;

        /// <summary>
        /// When this value is reached, the productfeedreader will sleep for 1ms.
        /// Lower this variable to prevent CPU overusage.
        /// </summary>
        public static int TicksUntilSleep;

        /// <summary>
        /// The logger used in the program.
        /// </summary>
        public static Logger Logger;

        /// <summary>
        /// The settings used in the program.
        /// </summary>
        public static Dictionary<string,string> settings;

        public static int maxQueueSize;
    }
}
