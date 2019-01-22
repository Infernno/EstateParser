using System.Collections.Specialized;
using EstateParser.Contracts.Data;

namespace EstateParser.Core.Models
{
    public class FullDataItem : IEstateFullDataItem
    {
        public string[] Images { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Contact { get; set; }
        public NameValueCollection Properties { get; set; }

        public FullDataItem(string[] images, string title, string description, string location, string contact, NameValueCollection properties)
        {
            Images = images;
            Title = title;
            Description = description;
            Location = location;
            Contact = contact;
            Properties = properties;
        }
    }
}
