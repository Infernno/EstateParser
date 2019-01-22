using EstateParser.Contracts.Data;

namespace EstateParser.Core.Models
{
    public class ShortDataItem : IEstateShortDataItem
    {
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public string Price { get; set; }

        public string Link { get; set; }

        public string ProviderName { get; set; }

        public ShortDataItem(string imageUrl, string title, string location, string price, string link, string providerName)
        {
            ImageUrl = imageUrl;
            Title = title;
            Location = location;
            Price = price;
            Link = link;
            ProviderName = providerName;
        }
    }
}
