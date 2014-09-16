using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductFeedReader
{
    public class Util
    {        
        /// <summary>
        /// This method will concatinate two string arrays.
        /// </summary>
        /// <param name="x">The first string array</param>
        /// <param name="y">The second string array</param>
        /// <returns>A concatination of both arrays.</returns>
        public static string[] ConcatArrays(string[] x, string[] y)
        {
            //Throw exceptions if one of the arguments is null.
            if (x == null) throw new ArgumentNullException("First argument is null.");
            if (y == null) throw new ArgumentNullException("Second argument is null.");

            //Save the length of x
            int oldLen = x.Length;

            //Resize x to be big enough to store all the values of y
            Array.Resize<string>(ref x, x.Length + y.Length);

            //Copy all elements of y into x
            Array.Copy(y, 0, x, oldLen, y.Length);

            //Return x
            return x;
        }

        public static string ToLiteral(string input)
        {
            StringBuilder literal = new StringBuilder(input.Length + 2);
            literal.Append("\"");
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'': literal.Append(@"\'"); break;
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                            // As UTF16 escaped character
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            literal.Append("\"");
            return literal.ToString();
        }
    }
}
