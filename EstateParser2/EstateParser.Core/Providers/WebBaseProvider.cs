using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using EstateParser.Contracts;
using EstateParser.Contracts.Data;
using EstateParser.Contracts.Providers;
using EstateParser.Contracts.Services;
using EstateParser.Core.Tools;
using HtmlAgilityPack;

// ReSharper disable VirtualMemberCallInConstructor

namespace EstateParser.Core.Providers
{
    public abstract class WebBaseProvider : IDataProvider
    {
        #region Properties

        public virtual string Name => GetType().Name.Replace("Provider", string.Empty);

        public virtual ProviderInfo Info { get; }

        public abstract int MaxOffset { get; }

        protected virtual string PathTemplate { get; } = ".//{0}[@class=\"{1}\"]";

        protected virtual string DefaultImageUrl { get; }

        protected virtual Encoding DefaultEncoding { get; } = Encoding.UTF8;

        protected abstract int ItemsPerPage { get; set; }

        protected abstract string BaseUrl { get; }

        protected abstract string RequestUrl { get; }

        protected abstract string ItemsClass { get; }

        protected IConfiguration Configuration { get; }

        protected IWebService WebService { get; }

        #endregion

        #region Constructor

        protected WebBaseProvider(IConfiguration configuration, IWebService webService)
        {
            Configuration = configuration;
            WebService = webService;

            DefaultImageUrl = configuration.Get("DefaultImageUrl");

            Info = new ProviderInfo(MaxOffset, ItemsPerPage * MaxOffset);
        }

        #endregion

        #region Public methods

        public async Task<IEstateFullDataItem> GetItemAsync(object arg, CancellationToken cancellationToken)
        {
            var url = arg.ToString();

            if (!url.Contains(BaseUrl))
                throw new InvalidOperationException($"Can't parse '{url}' using provider '{Name}'");

            var document = await DownloadDocumentAsync(url, cancellationToken);
            var item = await Task.Run(() => ParseFullItem(document), cancellationToken);

            return item;
        }

        public async Task<IEstateShortDataItem[]> GetItemsAsync(int count, int offset, CancellationToken cancellationToken)
        {
            var requestCount = Helper.DivideAndRound(count, ItemsPerPage);
            var requestOffset = offset > MaxOffset ? Helper.DivideAndRound(offset, ItemsPerPage) : 0;

            if (offset > MaxOffset)
            {
                throw new ArgumentException($"Failed to get data using '{Name}' provider." +
                                            $"Reason: offset > MaxOffset ({offset} > {MaxOffset})." +
                                            $"Parameters: count = {count}, offset = {offset}, items per page = {ItemsPerPage}");
            }

            if (requestCount <= 0)
            {
                throw new InvalidOperationException($"Failed to get data using '{Name}' provider." +
                                                    $"Reason: Request count <= 0 ({requestCount} <= 0)." +
                                                    $"Parameters: count = {count}, offset = {offset}, items per page = {ItemsPerPage}");
            }

            if (requestCount > 1)
            {
                var urls = GetRequestList(requestCount, requestOffset);

                Debug.WriteLine($"URL request count: {urls.Length}");
                Debug.WriteLine($"Total item count: {urls.Length * ItemsPerPage}");

                var data = await WebService.ExecuteBatchQuery(urls, DownloadItems, cancellationToken).ConfigureAwait(false);
                var result = data.SelectMany(x => x).ToArray();

                if (result.Length > count)
                    return result.Take(count).ToArray();

                return result.ToArray();
            }

            var url = string.Format(RequestUrl, requestOffset);
            var items = await DownloadItemsAsync(url, cancellationToken);

            if (items.Length > count)
                return items.Take(count).ToArray();

            return items;
        }

        public async Task<IEstateFullDataItem[]> GetFullItemsAsync(string[] args, CancellationToken cancellationToken)
        {
            var urls = args.Select(a => a.ToString()).ToArray();
            var data = await WebService.ExecuteBatchQuery(urls, DownloadItem, cancellationToken);

            return data;
        }

        #endregion

        #region Protected methods

        protected abstract IEstateFullDataItem ParseFullItem(HtmlDocument document);
        protected abstract IEstateShortDataItem ParseShortItem(HtmlNode node);

        protected virtual IEnumerable<IEstateShortDataItem> SelectItems(HtmlDocument document, CancellationToken token)
        {
            var nodes = document.DocumentNode.SelectNodes(ItemsClass);

            if (nodes == null || nodes.Count <= 0)
                throw new InvalidOperationException($"Failed to get data using '{Name}' provider." +
                                                    $"Reason: nodes == null || nodes.Count <= 0 ({nodes?.Count}).");

            var items = nodes.AsParallel()
                    .WithCancellation(token)
                    .Select(ParseShortItem);

            return items;
        }

        #endregion

        #region Private methods

        private IEstateFullDataItem DownloadItem(string url)
        {
            if (!url.Contains(BaseUrl))
                throw new InvalidOperationException($"Can't parse '{url}' using provider '{Name}'");

            using (var stream = WebService.DownloadStream(url))
            {
                var document = new HtmlDocument();

                document.Load(stream, DefaultEncoding);

                return ParseFullItem(document);
            }
        }

        private IEstateShortDataItem[] DownloadItems(string url)
        {
            using (var stream = WebService.DownloadStream(url))
            {
                var document = new HtmlDocument();

                document.Load(stream, DefaultEncoding);

                return SelectItems(document, CancellationToken.None).ToArray();
            }
        }

        private async Task<IEstateShortDataItem[]> DownloadItemsAsync(string url, CancellationToken cancellationToken)
        {
            var document = await DownloadDocumentAsync(url, cancellationToken);
            var items = SelectItems(document, cancellationToken).ToArray();

            return items;
        }

        private async Task<HtmlDocument> DownloadDocumentAsync(string url, CancellationToken token)
        {
            using (var stream = await WebService.DownloadStreamAsync(url, token))
            {
                var document = new HtmlDocument();

                document.Load(stream, DefaultEncoding);

                return document;
            }
        }

        private string[] GetRequestList(int count, int offset)
        {
            var urls = new List<string>();

            while (count-- > 0)
            {
                if (offset < 0 && offset > MaxOffset)
                    break;

                urls.Add(string.Format(RequestUrl, offset++));
            }

            return urls.ToArray();
        }

        #endregion
    }
}