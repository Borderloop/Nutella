using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Loggers;

namespace BorderSource.Common
{
    public class Statics
    {
        /// <summary>
        /// The logger used in the program.
        /// </summary>
        public static string LoggerPath;

        public static string AmiguousLogPath;

        public static QueryLogger SqlLogger;

        /// <summary>
        /// The settings used in the program.
        /// </summary>
        public static Dictionary<string,string> settings;

        public static Dictionary<string, int> DbPropertySizes;

        public static Dictionary<string, int> maxSizes;

        public static Dictionary<string, string> TwoWayDBProductToBobProductMapping;

        public static List<string> webshopNames;
    }
}
