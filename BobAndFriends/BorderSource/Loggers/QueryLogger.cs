using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BorderSource.Common;

namespace BorderSource.Loggers
{
    public class QueryLogger : StreamWriter
    {
        private Dictionary<string, string> _params;
        private StringBuilder _buffer;

        private bool selectQuery = false;
        public QueryLogger(string logPath) : base(logPath) 
        {
            _buffer = new StringBuilder();
            _params = new Dictionary<string, string>();
        }

        public override void Write(string value)
        {
            if (value.Contains("Started transaction") || value.Contains("Committed transaction") || value.Contains("Disposed transaction")) return;
            // Open connection - open _buffer
            if (value.Contains("Opened connection"))
            {
                selectQuery = false;
                return;
            }           

            // Get parameter values and replace them in the bufferstring.
            if (value.Contains("--"))
            {        
                // In comments now. Params are given with an "@".
                if (!value.Contains("@")) return;
                // Definitely a param now
                string param = value.Substring(2).Split(':')[0].Trim();
                string val = value.SplitFirstOnly(':')[1].Split('(')[0].Trim();
                if (_params.Keys.Contains(param))
                {
                    // Clearly we already have this param, meaning we have multiple updates in one transaction
                    foreach (KeyValuePair<string, string> pair in _params)
                    {
                        _buffer.Replace(pair.Key, pair.Value);
                    }
                    _params.Clear();
                    string buffer = _buffer.ToString().RemoveEscapedCharacters() == "" ? _buffer.ToString().RemoveEscapedCharacters() : "";
                    _buffer.Clear();
                    _buffer.Append(buffer);
                }
                _params.Add(param, val);
                return;
   
            }

            // Close connection - end of query
            if (value.Contains("Closed"))
            {
                if (selectQuery) return;
                foreach (KeyValuePair<string, string> pair in _params)
                {
                    _buffer.Replace(pair.Key, pair.Value);
                }
                base.Write(_buffer.ToString().RemoveEscapedCharacters() == "" ? _buffer.ToString().RemoveEscapedCharacters() : "");
                _buffer.Clear();
                _params.Clear();
                return;
            }

            // Only procede if it is not a select query
            if (value.Contains("SELECT")) selectQuery = true;
            else _buffer.Append(value + "; ");
        }

        public override void Close()
        {
            base.Close();
        }
    }
}
