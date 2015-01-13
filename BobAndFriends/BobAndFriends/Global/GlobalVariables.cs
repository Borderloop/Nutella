using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Common;
using System.Collections.Concurrent;

namespace BobAndFriends.Global
{
    public class GlobalVariables
    {
        public static ConcurrentHashSet<AffiliateIdEntry> UniqueIds { get; set; }
        public static ConcurrentHashSet<string> AddedCategorySynonyms { get; set; }
        public static ConcurrentDictionary<string, decimal> CurrencyRates { get; set; }
        public static ConcurrentDictionary<int, ConcurrentBag<string>> AddedProducts {get; set;}
        public static void Initialize()
        {
            UniqueIds = new ConcurrentHashSet<AffiliateIdEntry>();
            AddedCategorySynonyms = new ConcurrentHashSet<string>();
            CurrencyRates = new ConcurrentDictionary<string, decimal>(BorderSource.Web.CurrencyConverter.LiveCurrencyConverter.GetCurrencyRatesToEUR());
            AddedProducts = new ConcurrentDictionary<int, ConcurrentBag<string>>();
        }
    }

    public class AffiliateIdEntry : Object
    {
        public string Affiliate { get; set; }
        public string UniqueId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            AffiliateIdEntry entry = (AffiliateIdEntry)obj;
            return (entry.Affiliate == Affiliate) && (entry.UniqueId == UniqueId);
        }

        public override int GetHashCode()
        {
            return Affiliate.GetHashCode() ^ UniqueId.GetHashCode();
        }
    }
}
