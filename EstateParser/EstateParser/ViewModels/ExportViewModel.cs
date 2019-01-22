using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EstateParser.Contracts.Data;
using ReactiveUI;
using EstateParser.Contracts.Exporting;
using EstateParser.Contracts.Services;
using EstateParser.Core.Exporting;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ReactiveUI.Fody.Helpers;

namespace EstateParser.ViewModels
{
    public class ExportViewModel : ReactiveObject
    {
        #region Fields

        private IEstateDataService mDataService;
        private IExporter<EstateWorksheet> mExporter;

        #endregion

        #region UI Bindings

        public ISnackbarMessageQueue MessageQueue { get; }

        public ReactiveCommand Export { get; set; }

        public IEnumerable<string> Providers { get; set; }

        public IEnumerable<int> AdsCount => Enumerable.Range(1, mDataService.Provider.Info.MaxCount);

        [Reactive]
        public string ProviderSelection { get; set; }

        [Reactive]
        public int AdsToExport { get; set; }

        #endregion

        #region Constructor

        public ExportViewModel(IEstateDataService dataService, ISnackbarMessageQueue messageQueue)
        {
            mDataService = dataService;
            mExporter = new XlsxExporter();

            MessageQueue = messageQueue;

            Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            Export = ReactiveCommand.CreateFromTask(ExportData);

            Providers = mDataService.AllProviders.Select(p => p.Name);
            ProviderSelection = Providers.First();

            this.WhenAnyValue(x => x.ProviderSelection)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(AdsCount));
                });
        }

        private async Task ExportData()
        {
            MessageQueue.Enqueue("Экспорт успешно завершен!");

            return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Файлы Excel (*.xlsx) | *.xlsx"
            };

            var result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                MessageQueue.Enqueue("Загрузка объявлений, ждите...");

                var descriptions = await mDataService.Provider.GetItemsAsync(AdsToExport, 0, CancellationToken.None);

                var data = await mDataService.Provider.GetFullItemsAsync(descriptions.Select(d => d.Link).ToArray(),
                    CancellationToken.None);

                await Task.Run(() =>
                {
                    var worksheet = ToWorkSheet(data);

                    using (var fileStream = File.Create(saveFileDialog.FileName))
                    {
                        mExporter.Export(worksheet, fileStream);
                    }
                });

                MessageQueue.Enqueue("Экспорт успешно завершен!");
            }
        }

        private static EstateWorksheet ToWorkSheet(IEstateFullDataItem[] items)
        {
            return new EstateWorksheet(null, items);
        }

        #endregion
    }
}
