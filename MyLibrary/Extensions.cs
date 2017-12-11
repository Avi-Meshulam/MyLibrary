using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyLibrary
{
    static class Extensions
    {
        /// <summary>
        /// Splits a string with one space on every capital letter
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToString(this Enum input)
        {
            return input.ToString().SplitCamelCase();
        }

        public static string SplitCamelCase(this string input)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Replace(input, " ");
        }

        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            return items.Contains(item);
        }
    }
}
