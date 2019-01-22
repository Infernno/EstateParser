using System.Collections.Specialized;

namespace EstateParser.Contracts.Data
{
    /// <summary>
    /// Представляет собой базовый контракт модели полного объявления.
    /// <para>Содержит только основные и обязательные сведения об объявлении.</para>
    /// </summary>
    public interface IEstateFullDataItem
    {
        /// <summary>
        /// Изображение в объявлении, если имеются
        /// </summary>
        string[] Images { get; set; }

        /// <summary>
        /// Название в объявлении
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Описание в объявлении
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Локация (местонахождения)
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// Контактный адрес
        /// </summary>
        string Contact { get; set; }

        /// <summary>
        /// Остальные данные об объявлении
        /// </summary>
        NameValueCollection Properties { get; set; }
    }
}
