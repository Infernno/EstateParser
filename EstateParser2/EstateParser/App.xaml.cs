using System;
using System.Windows;
using EstateParser.Contracts;
using EstateParser.Contracts.Caching;
using EstateParser.Contracts.Exporting;
using EstateParser.Contracts.Providers;
using EstateParser.Contracts.Services;
using EstateParser.Core.Caching;
using EstateParser.Core.Exporting;
using EstateParser.Core.Providers;
using EstateParser.Core.Services;
using EstateParser.Infrastructure;
using EstateParser.ViewModels;
using MaterialDesignThemes.Wpf;
using SimpleInjector;

namespace EstateParser
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = ConfigureContainer();
            var viewModel = container.GetInstance<MainViewModel>();

            var window = new MainWindow();

            window.DataContext = viewModel;
            window.Show();
        }

        private static Container ConfigureContainer()
        {
            var container = new Container();

            RegisterProviders(container);
            RegisterServices(container);
            RegisterViewModels(container);

            return container;
        }

        private static void RegisterServices(Container container)
        {
            container.Register<IEstateDataService, EstateDataService>();
            container.Register<IWebService, WebService>(Lifestyle.Singleton);

            container.Register<IFileCache, FileCache>(Lifestyle.Singleton);

            container.Register<IExporter<EstateWorksheet>, XlsxExporter>();
            container.RegisterInstance<ISnackbarMessageQueue>(new SnackbarMessageQueue(TimeSpan.FromSeconds(5)));
            container.Register<IConfiguration, AppConfiguration>(Lifestyle.Singleton);
        }

        private static void RegisterProviders(Container container)
        {
            var providers = new[]
            {
                typeof(AvitoProvider),
                typeof(IrrRuProvider),
                typeof(AuRuProvider)
            };

            container.Collection.Register<IDataProvider>(providers);
        }

        private static void RegisterViewModels(Container container)
        {
            container.Register<MainViewModel>();
        }
    }
}
