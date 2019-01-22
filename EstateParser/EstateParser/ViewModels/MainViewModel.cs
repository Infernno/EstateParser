using System;
using System.Linq;
using System.Windows;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using EstateParser.Contracts.Providers;
using EstateParser.Contracts.Services;
using EstateParser.Core.Tools;
using EstateParser.Infrastructure;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EstateParser.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        #region Fields

        private ExportWindow mExportWindow;

        private readonly IEstateDataService mDataService;
        private readonly SearchFilter mSearchFilter;

        private IEnumerable<ShortItemViewModel> mCache;

        #endregion

        #region Properties

        public ReactiveCommand<Unit, IEnumerable<ShortItemViewModel>> Load { get; private set; }

        public ReactiveCommand Search { get; private set; }

        public ReactiveCommand Export { get; private set; }

        public bool IsLoading
        {
            get => ProgressBarVisibility == Visibility.Visible;
            set
            {
                if (value)
                {
                    ProgressBarVisibility = Visibility.Visible;
                    ItemsVisibility = Visibility.Hidden;
                }
                else
                {
                    ProgressBarVisibility = Visibility.Hidden;
                    ItemsVisibility = Visibility.Visible;
                }
            }
        }

        private IDataProvider DataProvider => mDataService.Provider;

        #endregion

        #region UI Bindings

        public ISnackbarMessageQueue MessageQueue { get; }

        [Reactive]
        public IEnumerable<ShortItemViewModel> Items { get; set; }

        [Reactive]
        public Visibility ProgressBarVisibility { get; set; }

        [Reactive]
        public Visibility ItemsVisibility { get; set; }

        [Reactive]
        public IEnumerable<string> Providers { get; set; }

        [Reactive]
        public string ProviderSelection { get; set; }

        [Reactive]
        public string RequiredTitle { get; set; }

        [Reactive]
        public string RequiredCity { get; set; }

        [Reactive]
        public string RequiredPriceBegin { get; set; }

        [Reactive]
        public string RequiredPriceEnd { get; set; }

        [Reactive]
        public List<int> Pages { get; set; }

        [Reactive]
        public int PageSelection { get; set; }

        #endregion

        #region Constructor

        public MainViewModel(IEstateDataService dataService, ISnackbarMessageQueue messageQueue)
        {
            mDataService = dataService;
            MessageQueue = messageQueue;

            mCache = new List<ShortItemViewModel>();
            mSearchFilter = new SearchFilter(() => new SearchRequest(RequiredTitle, RequiredCity, RequiredPriceBegin.AsLong(), RequiredPriceEnd.AsLong()));

            Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            Providers = mDataService.AllProviders.Select(p => p.Name);
            ProviderSelection = DataProvider.Name;

            Pages = Enumerable.Range(1, DataProvider.Info.MaxOffset).ToList();

            InitializeUI();

            PageSelection = 1;
        }

        private void InitializeUI()
        {
            // Load command
            Load = ReactiveCommand.CreateFromTask<Unit, IEnumerable<ShortItemViewModel>>((u, t) => LoadItems(t));

            Load.ObserveOn(SynchronizationContext.Current)
                .Subscribe(results =>
                {
                    mCache = results;

                    UpdateItems();

                    IsLoading = false;
                });

            Load.ThrownExceptions.Subscribe(ex =>
            {
                IsLoading = false;
                MessageQueue.Enqueue($"Возникла ошибка при загрузке данных - {ex.Message}");
                Debug.WriteLine("Ошибка при загрузке - " + ex);
            });

            this.WhenAnyValue(x => x.PageSelection, x => x.ProviderSelection)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(t => Unit.Default)
                .Do(request =>
                {
                    IsLoading = true;

                    if (DataProvider.Name != ProviderSelection)
                        mDataService.SetProvider(ProviderSelection);

                    Pages = Enumerable.Range(1, DataProvider.Info.MaxOffset).ToList();
                    PageSelection = Math.Min(PageSelection, Pages.Last());
                })
                .InvokeCommand(Load);

            // Search command
            Search = ReactiveCommand.Create(UpdateItems);

            this.WhenAnyValue(x => x.RequiredCity,
                    x => x.RequiredPriceBegin,
                    x => x.RequiredPriceEnd,
                    x => x.RequiredTitle)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(t => Unit.Default)
                .ObserveOn(SynchronizationContext.Current)
                .InvokeCommand(Search);

            // Export command
            Export = ReactiveCommand.Create(ExportData);

            Export.ThrownExceptions.Subscribe(ex =>
            {
                MessageQueue.Enqueue($"Возникла ошибка при экспорте данных - {ex.Message}");
            });
        }

        private void UpdateItems()
        {
            if (mCache.Any())
            {
                Items = new List<ShortItemViewModel>(mSearchFilter.CanApply
                    ? mCache.Where(i => mSearchFilter.Match(i))
                    : mCache);
            }
        }

        private async Task<IEnumerable<ShortItemViewModel>> LoadItems(CancellationToken token)
        {
            int count = DataProvider.Info.MaxCount / DataProvider.Info.MaxOffset;
            int offset = PageSelection;

            var data = await DataProvider.GetItemsAsync(count, offset, token);
            var items = data.ToArray();

            if (items.Length >= 100)
            {
                return items.AsParallel().Select(ShortItemViewModel.FromData);
            }

            return items.Select(ShortItemViewModel.FromData);
        }

        private void ExportData()
        {
            if (mExportWindow == null)
            {
                mExportWindow = new ExportWindow();

                mExportWindow.Owner = Application.Current.MainWindow;
                mExportWindow.DataContext = new ExportViewModel(mDataService, MessageQueue);

                mExportWindow.Closed += (sender, args) => { mExportWindow = null; };

                mExportWindow.Show();
            }
        }

        #endregion
    }
}