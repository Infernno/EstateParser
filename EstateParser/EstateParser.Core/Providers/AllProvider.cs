using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EstateParser.Contracts.Data;
using EstateParser.Contracts.Providers;
using EstateParser.Core.Tools;

namespace EstateParser.Core.Providers
{
    public class AllProvider : IDataProvider
    {
        public string Name { get; }

        public ProviderInfo Info { get; }

        public IDataProvider[] Providers { get; }

        public AllProvider(IEnumerable<IDataProvider> providers)
        {
            Name = "Все";
            Providers = providers.ToArray();

            var maxOffset = Providers.Sum(p => p.Info.MaxOffset);
            var maxCount = Providers.Sum(p => p.Info.MaxCount);

            Info = new ProviderInfo(maxOffset, maxCount);
        }

        public Task<IEstateFullDataItem> GetItemAsync(object arg, CancellationToken cancellationToken)
        {
            var url = arg.ToString();
            var provider = Providers.FirstOrDefault(p => url.ContainsIgnoreCase(p.Name.ToLower()));

            if (provider == null)
                throw new InvalidOperationException($"Can't load item at {url} because matching provider doesn't exist!");

            return provider.GetItemAsync(arg, cancellationToken);
        }

        public async Task<IEstateShortDataItem[]> GetItemsAsync(int count, int offset, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEstateShortDataItem[]>>();

            foreach (var provider in Providers)
            {
                var requestCount = Math.Min(count, provider.Info.MaxCount);
                var requestOffset = Math.Min(offset, provider.Info.MaxOffset);

                var task = provider.GetItemsAsync(requestCount, requestOffset, cancellationToken);

                tasks.Add(task);
            }

            var data = await Task.WhenAll(tasks);
            var result = data.SelectMany(x => x)
                .Shuffle()
                .ToArray();

            return result;
        }

        public Task<IEstateFullDataItem[]> GetFullItemsAsync(string[] args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
