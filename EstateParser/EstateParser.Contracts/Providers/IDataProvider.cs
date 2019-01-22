using System.Threading;
using System.Threading.Tasks;
using EstateParser.Contracts.Data;

namespace EstateParser.Contracts.Providers
{
    /// <summary>
    /// Интерфейс поставщика данных.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Название поставщика данных.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Данные о поставщике
        /// </summary>
        ProviderInfo Info { get; }

        /// <summary>
        /// Возвращает задачу <see cref="Task{TResult}"/>, содержащее объявление в виде <see cref="IEstateShortDataItem"/>.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns></returns>
        Task<IEstateFullDataItem> GetItemAsync(object arg, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает массив объявлений <see cref="IEstateShortDataItem"/> в качестве асинхронной операции.
        /// </summary>
        /// <param name="count">Количество элементов, которое необходимо получить</param>
        /// <param name="offset">Cмещение, необходимое для выборки определенного подмножества объявлений (может использоваться как страница).</param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns></returns>
        Task<IEstateShortDataItem[]> GetItemsAsync(int count, int offset, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает массив объявлений <see cref="IEstateFullDataItem"/> в качестве асинхронной операции.
        /// </summary>
        /// <param name="args">Аргументы</param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns></returns>
        Task<IEstateFullDataItem[]> GetFullItemsAsync(string[] args, CancellationToken cancellationToken);
    }
}
