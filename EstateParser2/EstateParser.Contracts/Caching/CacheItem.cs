using System;

namespace EstateParser.Contracts.Caching
{
    public abstract class CacheItem<T>
    {
        public T Item { get; set; }

        public DateTime ExpirationTime { get; set; }

        protected CacheItem(T item, DateTime expirationTime)
        {
            Item = item;
            ExpirationTime = expirationTime;
        }
    }
}