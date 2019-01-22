using EstateParser.Contracts.Data;

namespace EstateParser.Core.Exporting
{
    public class EstateWorksheet
    {
        /// <summary>
        /// Заголовки в Excel файле
        /// </summary>
        public string[] Headers { get; set; }

        /// <summary>
        /// Основные данные
        /// </summary>
        public IEstateFullDataItem[] Items { get; set; }

        public EstateWorksheet(string[] headers, IEstateFullDataItem[] items)
        {
            Headers = headers;
            Items = items;
        }

        public static EstateWorksheet Create(IEstateFullDataItem[] items)
        {
            throw new System.NotImplementedException(nameof(Create));
        }
    }
}
