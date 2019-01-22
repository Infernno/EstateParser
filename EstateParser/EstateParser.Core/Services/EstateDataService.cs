using System;
using System.Collections.Generic;
using System.Linq;
using EstateParser.Contracts.Providers;
using EstateParser.Contracts.Services;
using EstateParser.Core.Providers;

namespace EstateParser.Core.Services
{
    public class EstateDataService : IEstateDataService
    {
        #region Fields

        private readonly List<IDataProvider> mDataProviders = new List<IDataProvider>();

        #endregion

        #region Properties

        public IDataProvider Provider { get; private set; }

        public IEnumerable<IDataProvider> AllProviders => mDataProviders;

        #endregion

        #region Constructor

        public EstateDataService(IDataProvider[] providers)
        {
            if (providers == null)
                throw new ArgumentNullException(nameof(providers));

            foreach (var provider in providers)
            {
                if (Provider == null)
                    Provider = provider;

                mDataProviders.Add(provider);
            }

            mDataProviders.Add(new AllProvider(providers));
        }

        #endregion

        #region IEstateDataService

        public void SetProvider(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var provider = mDataProviders.FirstOrDefault(p => p.Name == name);

            Provider = provider ?? throw new ArgumentException($"Provider {name} doesn't exist!");
        }

        #endregion
    }
}
