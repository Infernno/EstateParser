using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EstateParser.Contracts.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EstateParser.ViewModels
{
    public class ShortItemViewModel : ReactiveObject, IEstateShortDataItem
    {
        #region Fields

        private static readonly HttpClient mCacheClient = new HttpClient();

        #endregion

        #region Properties

        /// <summary>
        /// Изображения в объявлении, если есть
        /// </summary>
        [Reactive]
        public BitmapImage Preview { get; set; }

        /// <summary>
        /// Состояние отображения прогресс бара
        /// </summary>
        [Reactive]
        public Visibility ProgressVisibility { get; set; }

        /// <summary>
        /// Состояние отображения фотографии
        /// </summary>
        [Reactive]
        public Visibility ImageVisibility { get; set; }

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Название объявление
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Локакация объявления (регион, город, район и т.п.)
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Цена, если указана
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Ссылка на объявление
        /// </summary>
        public string Link { get; set; }
        
        /// <summary>
        /// Открывает детальное описание об элементе
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// Состояние индикатора загрузки
        /// </summary>
        public bool IsLoading
        {
            get => ProgressVisibility == Visibility.Visible;
            set
            {
                if (value)
                {
                    ProgressVisibility = Visibility.Visible;
                    ImageVisibility = Visibility.Hidden;
                }
                else
                {
                    ProgressVisibility = Visibility.Hidden;
                    ImageVisibility = Visibility.Visible;
                }
            }
        }

        #endregion

        public ShortItemViewModel(string imageUrl, string title, string location, string price, string link)
        {
            Title = title;
            Location = location;
            Price = price;
            Link = link;
            ImageUrl = imageUrl;

            OpenCommand = ReactiveCommand.Create(() => Process.Start(Link));

            DownloadImage();
        }

        private async Task DownloadImage()
        {
            IsLoading = true;

            var data = await mCacheClient.GetByteArrayAsync(ImageUrl);
            var image = new BitmapImage();

            using (var memoryStream = new MemoryStream(data))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();

                if (image.CanFreeze && !image.IsFrozen)
                    image.Freeze();
            }

            Preview = image;

            IsLoading = false;
        }

        public static ShortItemViewModel FromData(IEstateShortDataItem item)
        {
            return new ShortItemViewModel(item.ImageUrl,
                            item.Title,
                            item.Location,
                            item.Price ?? "Цена не указана",
                            item.Link);
        }
    }
}
