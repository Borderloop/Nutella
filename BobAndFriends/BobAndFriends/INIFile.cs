using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BobAndFriends
{
    public class INIFile
    {
        /// <summary>
        /// The StreamReader which reads the .ini file
        /// </summary>
        private StreamReader _sr;

        /// <summary>
        /// The constructor which opens the StreamReader to read a .ini file from a given path.
        /// </summary>
        /// <param name="path">The path where the .ini file is located.</param>
        public INIFile(string path)
        {
            //Create the StreamReader
            _sr = new StreamReader(path);
        }

        /// <summary>
        /// This method will return the value of the searched property in the given section
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="section">The name of the section</param>
        /// <returns>The value of the property name if it exists, null otherwise/</returns>
        public string GetValue(string name, string section)
        {
            //Create an empty string
            string s;

            //Search for the given section.
            while(!(s =_sr.ReadLine()).Equals("[" + section + "]"))
            {
                //Section found - search for the given propertyname
                while(!(s = _sr.ReadLine()).Contains(name))
                {
                    //Return the value of the property
                    return s.Split('=')[1];
                }
            }

            //Didn't find any, therefore return null
            return null;
        }

        /// <summary>
        /// This method will return all values from a given section in a dictionary
        /// </summary>
        /// <param name="section">The name of the section</param>
        /// <returns>A dictionary containing propertynames and values.</returns>
        public Dictionary<string, string> GetAllValuesFromSection(string section)
        {
            //Create an empty string for reading, value and name.
            string s;
            string name;
            string val;

            //Create a dictionary
            Dictionary<string, string> map = new Dictionary<string, string>();

            //Start reading
            while (!_sr.EndOfStream)
            {
                //Check if the section is found
                if ((s = _sr.ReadLine()).Equals(@"[" + section + "]"))
                {
                    //Section is found - read all the values
                    while ((s = _sr.ReadLine() ?? "").Contains("="))
                    {
                        //Store values in strings
                        name = s.Split('=')[0];
                        val = s.Split('=')[1];

                        //Save values in the dictionary
                        map.Add(name, val);
                    }
                }
            }

            //Return the dictionary
            return map;
        }

        /// <summary>
        /// This method will return a dictionary with all properties and their values.
        /// </summary>
        /// <returns>A dictionary containing all properties and their values.</returns>
        public Dictionary<string, string> GetAllValues()
        {
            //Create an empty string for reading, value and name.
            string s;
            string name;
            string val;

            //Create a dictionary
            Dictionary<string, string> map = new Dictionary<string, string>();

            //Start reading
            while (!_sr.EndOfStream)
            {
                //Save values for every property found
                while ((s = _sr.ReadLine() ?? "").Contains("="))
                {
                    //However, do not read comments
                    if(s.Contains("#"))
                    {
                        continue;
                    }

                    //Store values in strings
                    name = s.Split('=')[0];
                    val = s.Split('=')[1];

                    //Save values in dictionary
                    map.Add(name, val);
                }
            }

            //Return the dictionary
            return map;
        }
    }
}
