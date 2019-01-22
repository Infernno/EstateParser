namespace EstateParser.Contracts.Data
{
    /// <summary>
    /// Представляет собой базовый контракт модели объявления
    /// <para>Содержит только основные и обязательные сведения об объявлении</para>
    /// </summary>
    public interface IEstateShortDataItem
    {
        /// <summary>
        /// Изображение в объявлении, если имеются
        /// </summary>
        string ImageUrl { get; set; }

        /// <summary>
        /// Название объявление
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Локакация объявления (регион, город, район и т.п.)
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// Цена, если указана
        /// </summary>
        string Price { get; set; }

        /// <summary>
        /// Ссылка на объявление
        /// </summary>
        string Link { get; set; }
    }
}
