using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Bumper
{
    public class INIFile
    {
        private string _path;

        /// <summary>
        /// The constructor which opens the StreamReader to read a .ini fichier from a given path.
        /// </summary>
        /// <param name="path">The path where the .ini fichier is located.</param>
        public INIFile(string path)
        {
            _path = path;
        }

        /// <summary>
        /// This method will return the value of the searched property in the given section
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="section">The name of the section</param>
        /// <returns>The value of the property name if it exists, null otherwise/</returns>
        public string GetValue(string name, string section)
        {
            string str;

            using (StreamReader reader = new StreamReader(_path))
            {
                while (!(str = reader.ReadLine()).Equals("[" + section + "]"))
                {
                    while (!(str = reader.ReadLine()).Contains(name))
                    {
                        return str.Split('=')[1];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// This method will return all values from a given section in a dictionary
        /// </summary>
        /// <param name="section">The name of the section</param>
        /// <returns>A dictionary containing propertynames and values.</returns>
        public Dictionary<string, string> GetAllValuesFromSection(string section)
        {
            string str;
            string name;
            string val;

            Dictionary<string, string> map = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(_path))
            {
                while (!reader.EndOfStream)
                {
                    if ((str = reader.ReadLine()).Equals(@"[" + section + "]"))
                    {
                        while ((str = reader.ReadLine() ?? "").Contains("="))
                        {
                            name = str.Split('=')[0];
                            val = str.Split('=')[1];

                            map.Add(name, val);
                        }
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// This method will return a dictionary with all properties and their values.
        /// </summary>
        /// <returns>A dictionary containing all properties and their values.</returns>
        public Dictionary<string, string> GetAllValues()
        {
            string str;
            string name;
            string val;

            Dictionary<string, string> map = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(_path))
            {
                while (!reader.EndOfStream)
                {
                    while ((str = reader.ReadLine() ?? "").Contains("="))
                    {
                        //Skip comments
                        if (str.Contains("#")) continue;
                        
                        name = str.Split('=')[0];
                        val = str.Split('=')[1];

                        map.Add(name, val);
                    }
                }
            }

            return map;
        }
    }
}
