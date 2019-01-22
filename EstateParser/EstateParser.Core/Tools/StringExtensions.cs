using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EstateParser.Core.Tools
{
    public static class StringExtensions
    {
        public static long AsLong(this string source)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))
                return 0;

            if (!source.All(char.IsDigit))
                source = Regex.Replace(source, "[^0-9]", string.Empty);

            return long.TryParse(source, out var value) ? value : 0;
        }

        public static int AsInt(this string source)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))
                return 0;

            if (!source.All(char.IsDigit))
                source = Regex.Replace(source, "[^0-9]", string.Empty);

            return int.TryParse(source, out var value) ? value : 0;
        }

        public static bool ContainsIgnoreCase(this string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            return !string.IsNullOrEmpty(source) && 
                   !string.IsNullOrEmpty(toCheck) && 
                   source.IndexOf(toCheck, comp) >= 0;
        }

        public static string StripHtml(this string source)
        {
            return Helper.StripHTML(source);
        }
    }
}
