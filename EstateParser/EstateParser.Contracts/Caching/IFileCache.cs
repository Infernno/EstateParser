namespace EstateParser.Contracts.Caching
{
    /// <inheritdoc />
    /// <summary>
    /// Файловый кэш
    /// </summary>
    public interface IFileCache : ICache<byte[]>
    {
        /// <summary>
        /// Директория с кэшем
        /// </summary>
        string CacheDirectory { get; }
    }
}
