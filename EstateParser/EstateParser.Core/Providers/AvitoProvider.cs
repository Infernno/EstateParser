using System;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using System.Diagnostics;
using EstateParser.Contracts;
using EstateParser.Contracts.Data;
using EstateParser.Contracts.Services;
using EstateParser.Core.Models;
using EstateParser.Core.Tools;
using HtmlAgilityPack;

namespace EstateParser.Core.Providers
{
    public class AvitoProvider : WebBaseProvider
    {
        #region Properties

        public override int MaxOffset { get; } = 100;

        protected override int ItemsPerPage { get; set; } = 35;

        protected override string BaseUrl { get; } = "https://www.avito.ru";

        protected override string RequestUrl { get; } = "https://www.avito.ru/rossiya/nedvizhimost?p={0}&view=gallery";

        protected override string ItemsClass =>
            string.Format(PathTemplate, "div", "item item_gallery js-catalog-item-enum");

        private string TitlePath =>
            string.Format(PathTemplate, "a", "description-title-link js-item-link");

        private string PricePath =>
            string.Format(PathTemplate, "span", "option price");

        private string LocationPath =>
            string.Format(PathTemplate + "//span", "div", "fader item_gallery-ellipsis");

        private string LinkPath =>
            string.Format(PathTemplate, "a", "img-link js-item-link");

        private string TitleDetailPath =>
            string.Format(PathTemplate, "span", "title-info-title-text");

        private readonly string[] ImagePaths =
        {
            ".//noscript//img",
            ".//img"
        };

        private string[] DescriptionDetailPaths => new[]
        {
            string.Format(PathTemplate + "//p", "div", "item-description-text"),
            string.Format(PathTemplate, "div", "item-description-html")
        };

        private string[] PropertiesDetailPath => new[]
        {
            string.Format(PathTemplate, "li", "item-params-list-item"),
            string.Format(PathTemplate + "//span", "div", "item-params"),
        };

        #endregion

        #region Constructor

        public AvitoProvider(IConfiguration configuration, IWebService webService) : base(configuration, webService)
        {
        }

        #endregion

        #region Methods

        protected override IEstateFullDataItem ParseFullItem(HtmlDocument document)
        {
            var root = document.DocumentNode;

            var titleNode = root.SelectSingleNode(TitleDetailPath);

            var descriptionNode = DescriptionDetailPaths.Select(root.SelectSingleNode)
                .First(node => node != null);

            var propertyNodes = PropertiesDetailPath.Select(root.SelectNodes)
                .First(nodes => nodes != null && nodes.Count > 0);

            var title = titleNode.InnerText.StripHtml();
            var description = descriptionNode.InnerText.StripHtml();

            var properties = new NameValueCollection();
            var builder = new StringBuilder();

            builder.AppendLine($"Название: {title}");
            builder.AppendLine($"Описание: {description}");

            foreach (var property in propertyNodes)
            {
                var values = property.InnerText.Split(':').Select(Helper.StripHTML).ToArray();

                var name = values.First();
                var value = values.Last();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                {
                    Debug.WriteLine($"Ignored: {name}");
                    continue;
                }

                builder.AppendLine($"{name} - {value}");

                properties.Add(name, value);
            }

            Debug.WriteLine(builder.ToString());
            Debugger.Break();

            throw new NotImplementedException();
        }

        protected override IEstateShortDataItem ParseShortItem(HtmlNode node)
        {
            // Nodes
            var titleNode = node.SelectSingleNode(TitlePath);
            var linkNode = node.SelectSingleNode(LinkPath);
            var locationNode = node.SelectSingleNode(LocationPath);
            var priceNode = node.SelectSingleNode(PricePath);

            // Text
            var title = titleNode?.InnerText.StripHtml();
            var location = locationNode?.InnerText.StripHtml();
            var price = priceNode?.InnerText.StripHtml();
            var link = BaseUrl + linkNode.Attributes["href"].Value;

            var imageUrl = Helper.FixLink(ExtractImageUrl(node));

            return new ShortDataItem(imageUrl, title, location, price, link, Name);
        }

        private string ExtractImageUrl(HtmlNode parent)
        {
            foreach (var imagePath in ImagePaths)
            {
                var node = parent.SelectSingleNode(imagePath);

                if (node != null && node.HasAttributes)
                {
                    return node.Attributes["src"].Value;
                }
            }

            return DefaultImageUrl;
        }

        #endregion
    }
}
