﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.EntityClient;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.ComponentModel;


namespace BorderSource.Common
{
    public static class Util
    {        
        /// <summary>
        /// This method will concatinate two string arrays.
        /// </summary>
        /// <param name="x">The first string array</param>
        /// <param name="y">The second string array</param>
        /// <returns>A concatination of both arrays.</returns>
        public static string[] ConcatArrays(string[] x, string[] y)
        {
            // Throw exceptions if one of the arguments is null.
            if (x == null) throw new ArgumentNullException("First argument is null.");
            if (y == null) throw new ArgumentNullException("Second argument is null.");

            // Save the length of x
            int oldLen = x.Length;

            // Resize x to be big enough to store all the values of y
            Array.Resize<string>(ref x, x.Length + y.Length);

            // Copy all elements of y into x
            Array.Copy(y, 0, x, oldLen, y.Length);

            // Return x
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
                        //  ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                            //  As UTF16 escaped character
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

        public static string RemoveEscapedCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            string[] parts = str.Split(new char[] {'\n', '\t', '\r', '\f', '\v', '\\' });
            foreach (string s in parts)
                sb.Append(s);
            return sb.ToString();
        }

        public static string EscapeChars(this string input)
        {
            char[] chars = { '.', '/', '\\', '>', '<', ':', '*', '?', '\"', '|' };
            foreach (char c in chars) input.Replace(c, ' ');
            return input;
        }

        public static string EmptyNull(this string str) { return str ?? ""; }

        public static string ToSHA256(this string str)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(str));
                foreach (Byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        public static bool Contains(this string source, string input, StringComparison comp)
        {
            return source.IndexOf(input, comp) >= 0;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            //  Create the result table, and gather all properties of a T        
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //  Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                //  Is it a nullable type? Get the underlying type 
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, propType);
            }

            //  Add the property values per T as rows to the datatable
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);

                table.Rows.Add(values);
            }

            return table;
        }

        /// <summary>
        ///   Converts a single DbDataRecord object into something else.
        ///   The destination type must have a default constructor.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "record"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this DbDataRecord record) where T : new()
        {
            var item = new T();
            for (int f = 0; f < record.FieldCount; f++)
            {
                var p = item.GetType().GetProperty(record.GetName(f));
                if (p != null && p.PropertyType == record.GetFieldType(f))
                {
                    p.SetValue(item, record.GetValue(f), null);
                }
            }

            return item;
        }

        public static string Truncate(this string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string ConcatAllButFirst(this string[] str)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string s in str)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static string[] SplitFirstOnly(this string str, char c)
        {
            string[] splitted = str.Split(c);
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < splitted.Length; i++)
            {
                sb.Append(splitted[i] + c);
            }
            sb.Remove(sb.Length - 1, 1);
            return new string[] {splitted[0], sb.ToString()};
        }

        public static string[] SplitAllButFirst(this string str, char c)
        {
            string[] splitted = str.Split(c);
            string[] allButFirst = new string[splitted.Count() - 1];
            for(int i = 1; i < allButFirst.Count(); i++)
            {
                allButFirst[i] = splitted[i].Trim();
            }
            return allButFirst;
        }

        public static int GetCumulativeValuesFromRange(this int[] array, int start, int end)
        {
            int sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        public static string RemoveDiacriticAccents(this string s)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(s);
            return System.Text.Encoding.UTF8.GetString(tempBytes);
        }

        public static T Clone<T>(this T t)
        {
            return (T)t.GetType().GetConstructor(new Type[]{}).Invoke(null);
        }

        public static string Swap(this string input, char firstChar, char secondChar)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in input)
            {
                if (c == firstChar) builder.Append(secondChar);
                else if (c == secondChar) builder.Append(firstChar);
                else builder.Append(c);
            }
            return builder.ToString();
        }

    }
}
