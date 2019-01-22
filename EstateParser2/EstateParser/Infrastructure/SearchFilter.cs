using System;
using EstateParser.Contracts.Data;
using EstateParser.Core.Tools;

namespace EstateParser.Infrastructure
{
    public class SearchFilter : AbstractFilter<IEstateShortDataItem, SearchRequest>
    {
        public SearchFilter(Func<SearchRequest> factory) : base(factory)
        {
            Add((item, requst) => item.Title.ContainsIgnoreCase(requst.Title), request => !string.IsNullOrWhiteSpace(request.Title));
            Add((item, requst) => item.Location.ContainsIgnoreCase(requst.Location), request => !string.IsNullOrWhiteSpace(request.Location));

            Add((item, requst) => item.Price.AsLong() >= requst.PriceRange.Minimum,
                request => request.PriceRange.Minimum > 0);

            Add((item, requst) => item.Price.AsLong() <= requst.PriceRange.Maximum,
                request => request.PriceRange.Maximum > 0 && request.PriceRange.Maximum > request.PriceRange.Minimum);
        }
    }
}