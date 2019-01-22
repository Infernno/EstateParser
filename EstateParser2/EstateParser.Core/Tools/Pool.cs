using System;
using System.Collections.Concurrent;

namespace EstateParser.Core.Tools
{
    public class Pool<T> where T : class, new()
    {
        #region Fields

        private const int StartCapacity = 6;

        private readonly object syncLock = new object();

        private readonly ConcurrentBag<T> mItems;
        private readonly Func<T> mFactory;

        private int mCapacity;

        #endregion

        #region Properties

        public int Capacity => mCapacity;
        public int Free => mItems.Count;

        #endregion

        #region Constructors

        public Pool() : this(StartCapacity, () => new T())
        {

        }

        public Pool(int capacity, Func<T> factory)
        {
            mItems = new ConcurrentBag<T>();

            mCapacity = capacity;
            mFactory = factory;

            for (int i = 0; i < mCapacity; i++)
            {
                mItems.Add(mFactory());
            }
        }

        #endregion

        #region Methods

        public void Release(T item)
        {
            mItems.Add(item);

            #if DEBUG
            lock (syncLock)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"{typeof(T).Name} returned to pool. Available - {Free}/{Capacity}");
            }
            #endif
        }

        public T Take()
        {
            if (TryGet(out var item))
                return item;

            throw new InvalidOperationException(nameof(Take));
        }

        public bool TryGet(out T item)
        {
            if (mItems.TryTake(out item))
            {
                #if DEBUG
                lock (syncLock)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"{typeof(T).Name} is taken from pool. Available - {Free}/{Capacity}");
                }
                #endif

                return true;
            }

            lock (syncLock)
            {
                item = mFactory();

                #if DEBUG
                System.Diagnostics.Debug.WriteLine(
                    $"{typeof(T).Name} is created in pool. Available - {Free}/{Capacity}");
                #endif

                mCapacity++;
            }

            return true;
        }
        
        #endregion
    }
}