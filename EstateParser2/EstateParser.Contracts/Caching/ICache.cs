namespace EstateParser.Contracts.Caching
{
    /// <summary>
    /// Контракт, предоставляющий кэширование данных
    /// </summary>
    /// <typeparam name="T">Элемент для кэширования</typeparam>
    public interface ICache<T>
    {
        /// <summary>
        /// Политика кэширования
        /// </summary>
        CachePolicy Policy { get; set; }

        /// <summary>
        /// Размер кэша (в байтах)
        /// </summary>
        long CacheSize { get; }

        /// <summary>
        /// Проверяет доступен ли кэш
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Включен ли кэш
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Кэширует элемент
        /// </summary>
        /// <param name="key">Название</param>
        /// <param name="item">Элемент</param>
        void Add(string key, T item);

        /// <summary>
        /// Возвращает элемент из кэша
        /// </summary>
        /// <param name="key">Название элемента</param>
        /// <returns></returns>
        CacheItem<T> Get(string key);

        /// <summary>
        /// Проверяет наличие элемента в кэше
        /// </summary>
        /// <param name="key">Название элемента</param>
        /// <returns>True - если элемент есть в кэше, False - в противоположном случае</returns>
        bool Contains(string key);

        /// <summary>
        /// Производит очистку кэша
        /// </summary>
        void Wipe();
    }
}
