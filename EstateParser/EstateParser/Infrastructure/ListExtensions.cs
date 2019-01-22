using System.Collections.Generic;
using ReactiveUI;

namespace EstateParser.Infrastructure
{
    public static class ListExtensions
    {
        public static void AddItems<T>(this ReactiveList<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }
    }
}
