using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BorderSource.Common
{
    public class QueryLogger : StreamWriter
    {
        private StringBuilder buffer;

        private bool selectQuery = false;
        public QueryLogger(string logPath) : base(logPath) { }

        public override void Write(string value)
        {
            //Open connection - open buffer
            if (value.Contains("Opened connection"))
            {
                buffer = new StringBuilder();
                return;
            }

            //Only procede if it is not a select query
            if (value.Contains("SELECT")) selectQuery = true;
            else buffer.Append(value);

            //Get parameter values and replace them in the bufferstring.
            if(value.Contains(@"-- @"))
            {
                string param;
                
            }

            //Close connection - end of query
            if (value.Contains("Closed connection"))
            {
                if (selectQuery) buffer.Clear();
                base.Write(buffer.ToString().RemoveEscapedCharacters());
                return;
            }


            buffer.Append(value);
        }

    }
}
