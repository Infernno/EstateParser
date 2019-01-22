using System;

namespace EstateParser.Contracts.Caching
{
    public class CachePolicy
    {
        public static readonly CachePolicy Default = new CachePolicy(TimeSpan.FromSeconds(5));

        public TimeSpan AbsoluteExpiration { get; }

        public CachePolicy(TimeSpan absoluteExpiration)
        {
            AbsoluteExpiration = absoluteExpiration;
        }
    }
}
