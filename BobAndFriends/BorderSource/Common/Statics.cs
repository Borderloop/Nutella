﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
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
        public static string LoggerPath;

        public static string AmiguousLogPath;

        public static QueryLogger SqlLogger;

        /// <summary>
        /// The settings used in the program.
        /// </summary>
        public static Dictionary<string,string> settings;

        public static Dictionary<string, int> DbPropertySizes;

        public static int maxQueueSize;

        public static int maxSkuSize;

        public static int maxBrandSize;

        public static int maxTitleSize;

        public static int maxImageUrlSize;

        public static int maxCategorySize;

        public static int maxShipTimeSize;

        public static int maxWebShopUrlSize;

        public static int maxDirectLinkSize;

        public static int maxAffiliateNameSize;

        public static int maxAffiliateProductIdSize;

        public static int maxResidueListSize;

        public static Dictionary<string, string> TwoWayDBProductToBobProductMapping;

        public static List<string> webshopNames;
    }
}
