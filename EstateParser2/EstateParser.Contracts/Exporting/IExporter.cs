using System.IO;

namespace EstateParser.Contracts.Exporting
{
    /// <summary>
    /// Контракт для экспорта данных
    /// </summary>
    /// <typeparam name="TItem">Элемент, который необходимо экспортировать</typeparam>
    public interface IExporter<in TItem>
    {
        /// <summary>
        /// Поддерживаемое расширение для экспорта
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Экспортирует элемент в указанный <see cref="Stream"/>
        /// </summary>
        /// <param name="item">Элемент для экспорта</param>
        /// <param name="stream"><see cref="Stream"/> (поток) для экспорта</param>
        void Export(TItem item, Stream stream);
    }
}
