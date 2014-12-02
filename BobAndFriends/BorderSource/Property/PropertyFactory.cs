using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace BorderSource.Property
{
    public static class PropertyFactory
    {
        private static Dictionary<string, IProperty> CreateDefaults()
        {
            Dictionary<string, IProperty> dic = new Dictionary<string, IProperty>();
            List<IProperty> propList = new List<IProperty>();
            #region String properties
            propList.Add(new Property<string>(){ DefaultValue = @"C:\BorderSoftware\BobAndFriends\log", PropertyName = "log_path"});
            propList.Add(new Property<string>() { DefaultValue = @"C:\BorderSoftware\Boris\ProductFeeds" , PropertyName = "feed_path"});
            propList.Add(new Property<string>() { DefaultValue = @"127.0.0.1", PropertyName = "db_source" });
            propList.Add(new Property<string>() { DefaultValue = "borderloop", PropertyName = "db_name" });
            propList.Add(new Property<string>() { DefaultValue = "root", PropertyName = "db_userid" });
            propList.Add(new Property<string>() { DefaultValue = "", PropertyName = "db_password" });                  
            #endregion

            #region Int32 properties
            propList.Add(new Property<int>() { DefaultValue = 3306, PropertyName = "db_port"});
            propList.Add(new Property<int>() { DefaultValue = 125, PropertyName = "db_max_pool_size" });
            propList.Add(new Property<int>() { DefaultValue = 10000, PropertyName = "max_packagequeue_size" });
            propList.Add(new Property<int>() { DefaultValue = 0, PropertyName = "max_validationqueue_size" });
            propList.Add(new Property<int>() { DefaultValue = 20, PropertyName = "max_sku_size" });
            propList.Add(new Property<int>() { DefaultValue = 100, PropertyName = "max_brand_size" });
            propList.Add(new Property<int>() { DefaultValue = 255, PropertyName = "max_title_size" });
            propList.Add(new Property<int>() { DefaultValue = 255, PropertyName = "max_imageurl_size" });
            propList.Add(new Property<int>() { DefaultValue = 60, PropertyName = "max_category_size" });
            propList.Add(new Property<int>() { DefaultValue = 60, PropertyName = "max_shiptime_size" });
            propList.Add(new Property<int>() { DefaultValue = 255, PropertyName = "max_webshopurl_size" });
            propList.Add(new Property<int>() { DefaultValue = 350, PropertyName = "max_directlink_size" });
            propList.Add(new Property<int>() { DefaultValue = 50, PropertyName = "max_affiliatename_size" });
            propList.Add(new Property<int>() { DefaultValue = 100, PropertyName = "max_affiliateproductid_size" });
            propList.Add(new Property<int>() { DefaultValue = 10, PropertyName = "max_bob_threads" });
            propList.Add(new Property<int>() { DefaultValue = 3, PropertyName = "max_reader_threads" });
            propList.Add(new Property<int>() { DefaultValue = 3, PropertyName = "max_bobbox_threads" });
            propList.Add(new Property<int>() { DefaultValue = 25, PropertyName = "package_size" });
            #endregion

            #region Boolean properties
            propList.Add(new Property<bool>() { DefaultValue = false, PropertyName = "log_enabled" });
            propList.Add(new Property<bool>() { DefaultValue = true, PropertyName = "insert_match_enabled" });
            propList.Add(new Property<bool>() { DefaultValue = true, PropertyName = "insert_new_enabled" });
            propList.Add(new Property<bool>() { DefaultValue = true, PropertyName = "update_enabled" });
            #endregion

            //Map to the dictionary based on their name
            foreach(IProperty prop in propList)
            {
                dic.Add(prop.PropertyName, prop);
            }

            return dic;
        }
        public static ConcurrentDictionary<string, IProperty> CreateFromFile(string path)
        {
            Console.WriteLine("Initializing properties...");
            Dictionary<string, IProperty> dic = CreateDefaults();
            INIFile ini = new INIFile(path);
            Dictionary<string, string> values = ini.GetAllValues();
            foreach(KeyValuePair<string, string> pair in values)
            {
                if (dic.ContainsKey(pair.Key))
                {
                    if (dic[pair.Key] is Property<int>)
                    {
                        int value;
                        if (!int.TryParse(pair.Value, out value)) continue;
                        ((Property<int>)dic[pair.Key]).CurrentValue = value;
                        dic[pair.Key].IsSet = true;
                    }

                    if (dic[pair.Key] is Property<string>)
                    {
                        ((Property<string>)dic[pair.Key]).CurrentValue = pair.Value;
                        dic[pair.Key].IsSet = true;
                    }

                    if (dic[pair.Key] is Property<bool>)
                    {
                        bool valueIsValid = (pair.Value == "true" || pair.Value == "false");
                        if (!valueIsValid) continue;
                        ((Property<bool>)dic[pair.Key]).CurrentValue = (pair.Value == "true");
                        dic[pair.Key].IsSet = true;
                    }
                }
            }
            ConcurrentDictionary<string, IProperty> cdic = new ConcurrentDictionary<string, IProperty>(dic);
            return cdic;
        }
    }
}
