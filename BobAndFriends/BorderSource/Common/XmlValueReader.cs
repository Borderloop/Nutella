using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using BorderSource.Loggers;

namespace BorderSource.Common
{
    public class XmlValueReader : IDisposable
    {
        public XmlReader _reader;
        public DualKeyDictionary<string, XmlNodeType, string> dkd;
        private string file;

        public string ProductEnd { get; set; }

        public XmlValueReader() 
        {           
            dkd = new DualKeyDictionary<string, XmlNodeType, string>();
        }

        public void CreateReader(string file)
        {
            _reader = XmlReader.Create(file);
            this.file = file;
        }

        public void CreateReader(string file, XmlReaderSettings settings)
        {
            _reader = XmlReader.Create(file, settings);
        }
        public void AddKeys(string key1, XmlNodeType key2)
        {
            //Add the keys to the dictionary with an empty value. This will be filled later.
            if (dkd.ContainsKey(key1, key2)) return;
            dkd.Add(key1, key2, "");
        }

        public IEnumerable<DualKeyDictionary<string, XmlNodeType, string>> ReadProducts()
        {
            if (_reader == null)
                throw new NullReferenceException("Reader is null. Call CreateReader() first.");
            bool isDone = false; ;
            bool nextProduct = false;
            while (!isDone)
            {
                try
                {
                    while (_reader.Read() && !nextProduct)
                    {
                        if (dkd.ContainsKey(_reader.Name, _reader.NodeType))
                        {
                            string key1 = _reader.Name;
                            XmlNodeType key2 = _reader.NodeType;
                            if (_reader.NodeType == XmlNodeType.Element)
                                _reader.Read(); //Point reader 1 step forward to get the actual data
                            if (_reader.NodeType == XmlNodeType.Text || _reader.NodeType == XmlNodeType.CDATA)
                                dkd.Add(key1, key2, _reader.Value); //Only read text or CDATA sections.
                        }
                        if (_reader.Name == ProductEnd && _reader.NodeType == XmlNodeType.EndElement)
                        {
                            nextProduct = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.WriteLine("File " + file + " threw an exception: " + e.Message);
                    Logger.Instance.WriteLine("StackTrace: " + e.StackTrace);
                }
                isDone = !nextProduct;
                nextProduct = false;
                yield return dkd;
                dkd.ClearValues();
            }
        }

        public void Dispose()
        {
            _reader.Close();
            _reader.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
