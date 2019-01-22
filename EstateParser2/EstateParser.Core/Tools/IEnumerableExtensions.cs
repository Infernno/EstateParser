using System;
using System.Collections.Generic;

namespace EstateParser.Core.Tools
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var list = new List<T>(source);
            var random = new Random((int)DateTime.Now.Ticks);

            int n = list.Count;

            while (n > 1)
            {
                n--;

                int k = random.Next(n + 1);
                T value = list[k];

                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
