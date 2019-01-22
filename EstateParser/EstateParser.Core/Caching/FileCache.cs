using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using EstateParser.Contracts.Caching;

namespace EstateParser.Core.Caching
{
    public class FileCacheItem : CacheItem<byte[]>
    {
        public FileCacheItem(byte[] item, DateTime expirationTime) : base(item, expirationTime)
        {
        }
    }

    public class FileCache : IFileCache
    {
        #region Fields

        private readonly ConcurrentDictionary<string, FileCacheItem> mItemCache;
        private readonly DirectoryInfo mCacheInfo;

        #endregion

        #region Properties

        public CachePolicy Policy { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsAvailable => IsEnabled && CacheSize > 0;

        public long CacheSize
        {
            get
            {
                if (Directory.Exists(CacheDirectory))
                {
                    return mCacheInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly)
                        .Sum(f => f.Length);
                }

                return 0;

            }
        }

        public string CacheDirectory { get; }

        #endregion

        #region Constructor

        public FileCache()
        {
            IsEnabled = false;

            Policy = CachePolicy.Default;
            CacheDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Cache");

            mItemCache = new ConcurrentDictionary<string, FileCacheItem>();
            mCacheInfo = new DirectoryInfo(CacheDirectory);

            if (Directory.Exists(CacheDirectory))
            {
                Wipe();
            }
            else if (IsEnabled)
            {
                Directory.CreateDirectory(CacheDirectory);
            }
        }

        #endregion

        #region Methods

        public void Add(string key, byte[] item)
        {
            if (!mItemCache.ContainsKey(key) && IsEnabled)
            {
                mItemCache.TryAdd(key, Create(key, item));

                DeleteOld();
            }
        }

        public CacheItem<byte[]> Get(string key)
        {
            if (!mItemCache.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key} doesn't exist!'");

            mItemCache[key].ExpirationTime = DateTime.Now.Add(Policy.AbsoluteExpiration);

            DeleteOld();

            return mItemCache[key];
        }

        public bool Contains(string key)
        {
            return IsEnabled && mItemCache.ContainsKey(key) && File.Exists(GetPath(key));
        }

        public void Wipe()
        {
            var files = Directory.GetFiles(CacheDirectory);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            mItemCache.Clear();
        }

        private void DeleteOld()
        {
            Task.Run(() =>
            {
                var keys = new List<string>();
                var currentDate = DateTime.Now;

                foreach (var item in mItemCache)
                {
                    if (item.Value.ExpirationTime < currentDate)
                    {
                        var path = GetPath(item.Key);

                        if (File.Exists(path))
                        {
                            try
                            {
                                File.Delete(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Failed to delete {path}: {ex}");
                            }
                        }

                        keys.Add(item.Key);
                    }
                }

                foreach (var key in keys)
                {
                    mItemCache.TryRemove(key, out _);
                }
            });
        }

        private string GetPath(string key)
        {
            return Path.Combine(CacheDirectory, key);
        }

        private FileCacheItem Create(string key, byte[] data)
        {
            var path = GetPath(key);

            File.WriteAllBytes(path, data);

            return new FileCacheItem(data, DateTime.Now.Add(Policy.AbsoluteExpiration));
        }

        #endregion
    }
}
