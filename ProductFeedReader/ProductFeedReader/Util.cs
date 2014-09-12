using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductFeedReader
{
    public class Util
    {
        public static int TickCount = 0;

        public static int SleepCount = 500000;      

        public static Logger Logger;
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
