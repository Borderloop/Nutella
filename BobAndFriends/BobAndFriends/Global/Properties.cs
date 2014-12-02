using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Property;
using System.Collections.Concurrent;

namespace BobAndFriends.Global
{
    public static class Properties
    {
        public static ConcurrentDictionary<string, IProperty> PropertyList = PropertyFactory.CreateFromFile(@"C:\BorderSoftware\BobAndFriends\settings\baf.ini");
    }
}
