namespace EstateParser.Contracts.Providers
{
    /// <summary>
    /// Данные о <see cref="IDataProvider"/>
    /// </summary>
    public class ProviderInfo
    {
        /// <summary>
        /// Максимальное смещение
        /// </summary>
        public int MaxOffset { get; set; }

        /// <summary>
        /// Максимальное количество элементов
        /// </summary>
        public int MaxCount { get; set; }

        public ProviderInfo(int maxOffset, int maxCount)
        {
            MaxOffset = maxOffset;
            MaxCount = maxCount;
        }
    }
}