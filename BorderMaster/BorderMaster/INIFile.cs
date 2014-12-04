using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BorderMaster
{
    public class INIFile
    {
        private string _path;

        public INIFile(string path)
        {
            _path = path;
        }

        /// <summary>
        /// This method will return all values from a given section in a dictionary
        /// </summary>
        /// <param name="section">The name of the section</param>
        /// <returns>A dictionary containing propertynames and values.</returns>
        public Dictionary<int, string> GetAllValuesFromSection(string section)
        {
            string str;
            int name;
            string val;

            Dictionary<int, string> map = new Dictionary<int, string>();

            using (StreamReader reader = new StreamReader(_path))
            {
                while (!reader.EndOfStream)
                {
                    if ((str = reader.ReadLine()).Equals(@"[" + section + "]"))
                    {
                        while ((str = reader.ReadLine() ?? "").Contains("="))
                        {
                            if (!Int32.TryParse(str.Split('=')[0], out name))
                                continue;

                            val = str.Split('=')[1];

                            map.Add(name, val);
                        }
                    }
                }
            }

            return map;
        }
    }
}

