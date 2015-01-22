﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Property;
using System.Collections.Concurrent;

namespace MasterGUI
{
    public static class Settings
    {
        public static ConcurrentDictionary<string, IProperty> PropertyList = new ConcurrentDictionary<string,IProperty>(PropertyFactory.CreateFromFile(@"C:\BorderSoftware\BobAndFriends\settings\baf.ini"));
    }
}