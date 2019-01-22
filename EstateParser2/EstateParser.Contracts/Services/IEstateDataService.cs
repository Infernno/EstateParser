using System.Collections.Generic;
using EstateParser.Contracts.Providers;

namespace EstateParser.Contracts.Services
{
    public interface IEstateDataService
    {
        /// <summary>
        /// Текущий поставщик данных
        /// </summary>
        IDataProvider Provider { get; }

        /// <summary>
        /// Возвращает последовательность, содержащую название всех поставщиков данных
        /// </summary>
        IEnumerable<IDataProvider> AllProviders { get; }

        /// <summary>
        /// Устанавливает текущего поставщика данных
        /// </summary>
        /// <param name="name">Название поставщика из <see cref="AllProviders"/></param>
        void SetProvider(string name);
    }
}
