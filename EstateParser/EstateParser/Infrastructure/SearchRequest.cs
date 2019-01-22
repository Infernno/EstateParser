using EstateParser.Core.Tools;

namespace EstateParser.Infrastructure
{
    public class SearchRequest
    {
        public string Title { get; set; }

        public string Location { get; set; }

        public Range<long> PriceRange { get; set; }

        public SearchRequest(string title, string location, long priceBegin, long priceEnd)
        {
            Title = title;
            Location = location;

            PriceRange = new Range<long>(priceBegin, priceEnd);
        }
    }
}