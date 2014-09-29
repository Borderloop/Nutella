using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BobAndFriends
{
    public class XmlValueReader
    {
        public XmlReader _reader;
        public DualKeyDictionary<string, XmlNodeType, string> dkd;

        public string ProductEnd { get; set; }

        public XmlValueReader() 
        {           
            dkd = new DualKeyDictionary<string, XmlNodeType, string>();
        }

        public void CreateReader(string file)
        {
            _reader = XmlReader.Create(file);
        }
        public void AddKeys(string key1, XmlNodeType key2)
        {
            //Add the keys to the dictionary with an empty value. This will be filled later.
            dkd.Add(key1, key2, "");
        }

        public IEnumerable<DualKeyDictionary<string, XmlNodeType, string>> ReadProducts()
        {
            if (_reader == null)
                throw new NullReferenceException("Reader is null. Call CreateReader() first.");

            while(_reader.Read())
            {                
                if(dkd.ContainsKey(_reader.Name, _reader.NodeType))
                {
                    string key1 = _reader.Name;
                    XmlNodeType key2 = _reader.NodeType;
                    if(_reader.NodeType == XmlNodeType.Element)
                        _reader.Read();
                    dkd.Add(key1, key2, _reader.Value);
                }
                if (_reader.Name == ProductEnd && _reader.NodeType == XmlNodeType.EndElement)
                {
                    yield return dkd;
                }
            }
        }

    }
}
