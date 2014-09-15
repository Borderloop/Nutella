using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductFeedReader
{
    public class Util
    {        
        public static string[] ConcatArrays(string[] x, string[] y)
        {
            if (x == null) throw new ArgumentNullException("First argument is null.");
            if (y == null) throw new ArgumentNullException("Second argument is null.");
            int oldLen = x.Length;
            Array.Resize<string>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }  
    }
}
