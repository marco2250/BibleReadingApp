using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BibleReading.Common45.Root
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static string GetValueOrDefault(this string s)
        {
            return s.IsNullOrEmpty() ? string.Empty : s;
        }

        public static T ToEnum<T>(this string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }

        public static string EndSubstring(this string s, int len)
        {
            if (s.IsNullOrEmpty() || s.Length < len)
                return string.Empty;
            return s.Substring(s.Length - len);
        }

        public static string ExtractNumbers(this string expr)
        {
            return string.Join(null, Regex.Split(expr, "[^\\d]"));
        }

        public static bool IsNaturalNumber(this string expr)
        {
            var oNum = new Regex(@"^[1-9]\d*\.?[0]*$"); 
            return oNum.IsMatch(expr);
        }

        public static string Capitalize(this string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            
            if (s.Length == 0)
                return s;

            var result = new StringBuilder(s);
            result[0] = char.ToUpper(result[0]);
            for (var i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
            }

            return result.ToString();
        }

        public static SortedList<string, string> GetSupportInfo(this string s)
        {
            var ret = new SortedList<string, string>();

            if (string.IsNullOrEmpty(s))
                return null;

            var parameters = s.Split('&');

            foreach (var param in parameters)
            {
                var name = param.Split('=')[0];
                var value = param.Split('=')[1];

                ret.Add(name, value);
            }

            return ret;
        }

        public static string ReplaceInsensitive(this string original, string pattern, string replacement)
        {
            int position0;
            int position1;
            var count = position0 = 0;
            var upperString = original.ToUpper();
            var upperPattern = pattern.ToUpper();
            var inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            var chars = new char[original.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0, StringComparison.Ordinal)) != -1)
            {
                for (var i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                foreach (var t in replacement)
                    chars[count++] = t;
                position0 = position1 + pattern.Length;
            }

            if (position0 == 0) return original;

            for (var i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            
            return new string(chars, 0, count);
        }
    }
}
