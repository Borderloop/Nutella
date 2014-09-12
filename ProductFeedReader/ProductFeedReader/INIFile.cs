using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProductFeedReader
{
    public class INIFile
    {
        private StreamReader _sr;

        public INIFile(string path)
        {
            _sr = new StreamReader(path);
        }

        public string GetValue(string name, string section)
        {
            string s;
            while(!(s =_sr.ReadLine()).Equals("[" + section + "]"))
            {
                while(!(s = _sr.ReadLine()).Contains(name))
                {
                    return s.Split('=')[1];
                }
            }
            return null;
        }

        public Dictionary<string, string> GetAllValuesFromSection(string section)
        {
            string s;
            string name;
            string val;

            Dictionary<string, string> map = new Dictionary<string, string>();

            while (!_sr.EndOfStream)
            {
                if ((s = _sr.ReadLine()).Equals(@"[" + section + "]"))
                {
                    while ((s = _sr.ReadLine() ?? "").Contains("="))
                    {
                        name = s.Split('=')[0];
                        val = s.Split('=')[1];
                        map.Add(name, val);
                    }
                }
            }
            return map;
        }

        public Dictionary<string, string> GetAllValues()
        {
            string s;
            string name;
            string val;

            Dictionary<string, string> map = new Dictionary<string, string>();

            while (!_sr.EndOfStream)
            {
                while ((s = _sr.ReadLine() ?? "").Contains("="))
                {
                    //Do not read comments
                    if(s.Contains("#"))
                    {
                        continue;
                    }

                    name = s.Split('=')[0];
                    val = s.Split('=')[1];
                    map.Add(name, val);
                }
            }
            return map;
        }
    }
}
