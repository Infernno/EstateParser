using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EstateParser.Contracts.Services
{
    /// <summary>
    /// Контракт для веб сервиса, осуществляющего загрузка данных
    /// </summary>
    public interface IWebService
    {
        /// <summary>
        /// Загружает страницу как поток байт
        /// </summary>
        /// <param name="url">URL адрес</param>
        /// <returns><see cref="Stream"/>, содержащий поток байтов</returns>
        Stream DownloadStream(string url);

        /// <summary>
        /// Загружает страницу как массив байт в качестве асинхронной операции
        /// </summary>
        /// <param name="url">URL адрес</param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns><see cref="Task{TResult}"/> с <see cref="Stream"/>, содержащий поток байт</returns>
        Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken);

        /// <summary>
        /// Выполняет пакетный запрос, преобразовывая данные из <see cref="TInput"/> в <see cref="TResult"/> в параллельном режиме в качестве асинхронной операции.
        /// Например, может использоваться для загрузки объявлений с большого количества страниц.
        /// </summary>
        /// <typeparam name="TInput">Входные данные для преобразования</typeparam>
        /// <typeparam name="TResult">Результат выполнения операции</typeparam>
        /// <param name="args">Входные данные. Например, ими могут выступать URL адреса страниц в интернете.</param>
        /// <param name="factory">Фабрика, создающая <see cref="TResult"/> из <see cref="TInput"/></param>
        /// <returns><see cref="Task"/>, содержащий массив <see cref="TResult"/></returns>
        Task<TResult[]> ExecuteBatchQuery<TInput, TResult>(TInput[] args, Func<TInput, TResult> factory);
    }
}
