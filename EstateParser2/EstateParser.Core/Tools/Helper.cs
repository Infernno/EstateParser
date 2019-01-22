using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EstateParser.Core.Tools
{
    public static class Helper
    {
        private static readonly string[] StripPatterns = { "&.*?;", "<.*?>" };
        private static readonly char[] StripChars = { ' ', '\n', '\t' };

        public static IEnumerable<Type> GetTypesOf(Type desiredType, bool onlyInstanceable = true)
        {
            if (desiredType == null)
                return null;

            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(desiredType.IsAssignableFrom);

            return onlyInstanceable ? types.Where(t => !t.IsAbstract && !t.IsInterface) : types;
        }

        public static string StripHTML(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return StripPatterns.Aggregate(text, (current, pattern) => Regex.Replace(current, pattern, string.Empty))
                .Trim(StripChars);
        }

        public static string FixLink(string url)
        {
            if (!string.IsNullOrEmpty(url) && url.StartsWith("//"))
            {
                return url.Insert(0, "https:");
            }

            return url;
        }

        public static int DivideAndRound(int a, int b)
        {
            return (a + b - 1) / b;
        }
    }
}
