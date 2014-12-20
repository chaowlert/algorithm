using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Chaow.Extensions
{
    public static class StringExt
    {
        public static string Join(this IEnumerable<string> source, string separator)
        {
            var value = source as string[];
            if (value == null)
                value = source.ToArray();
            return string.Join(separator, value);
        }

        public static string Reverse(this string text)
        {
            var charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string Repeat(this string source, int num)
        {
            if (num <= 0)
                return string.Empty;

            var sb = new StringBuilder(source.Length * num);
            for (var i = 0; i < num; i++)
                sb.Append(source);
            return sb.ToString();
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            return source.Select(s => s.ToString()).Join(separator);
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator, string format) where T : IFormattable
        {
            return source.Select(s => s.ToString(format, CultureInfo.CurrentCulture)).Join(separator);
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator, string format, IFormatProvider provider) where T : IFormattable
        {
            return source.Select(s => s.ToString(format, provider)).Join(separator);
        }
    }
}