using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Common;

namespace BobAndFriends.Global
{
    public class GlobalVariables
    {
        public static ConcurrentHashSet<string> UniqueIds = new ConcurrentHashSet<string>();
        public static ConcurrentHashSet<string> AddedCategorySynonyms = new ConcurrentHashSet<string>();
    }
}
