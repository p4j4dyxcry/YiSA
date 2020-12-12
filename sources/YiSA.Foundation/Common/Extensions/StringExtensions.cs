using System;
using System.IO;

namespace YiSA.Foundation.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TryAppendDoubleQuote(this string value)
        {
            if(value.Contains("\""))
                return value;
                
            return $"\"{value}\"";
        }

        public static string RemoveLastSeparator(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var last = value[value.Length - 1];
            if (last is '\\' || last is '/')
                return value.Substring(0, value.Length - 1);
           
            return value;
        }

        public static string ToNormalizePath(this string value)
        {
            return new Uri(Path.GetFullPath(value.RemoveLastSeparator())).ToString().Replace('\\','/');
        }

        public static bool Contains(this string value ,string target, StringComparison comparison)
        {
            return value.IndexOf(target, comparison) != -1;
        }
    }
}