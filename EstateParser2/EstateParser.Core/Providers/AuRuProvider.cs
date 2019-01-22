using System;
using EstateParser.Contracts;
using EstateParser.Contracts.Data;
using EstateParser.Contracts.Services;
using EstateParser.Core.Models;
using EstateParser.Core.Tools;
using HtmlAgilityPack;

namespace EstateParser.Core.Providers
{
    public class AuRuProvider : WebBaseProvider
    {
        #region Properties

        public override string Name { get; } = "Au.Ru";

        public override int MaxOffset { get; } = 200;

        protected override int ItemsPerPage { get; set; } = 50;

        protected override string BaseUrl { get; } = "https://au.ru";

        protected override string RequestUrl { get; } = "https://au.ru/auction/realty/?page={0}";

        protected override string ItemsClass =>
            string.Format(PathTemplate, "div", "au-lots-item au-card-list-item");

        private string ImagePath =>
            string.Format(PathTemplate, "img", "au-lot-card-pic__img");

        private string TitlePath =>
            string.Format(PathTemplate, "div", "au-item-name__link");

        private string PricePath =>
            string.Format(PathTemplate, "span", "au-price__value");

        private string LocationPath =>
            string.Format(PathTemplate, "div", "au-geo-mark__name");

        private string LinkPath =>
            string.Format(PathTemplate + "//a", "div", "au-item-name__link");


        #endregion

        #region Constructor

        public AuRuProvider(IConfiguration configuration, IWebService webService) : base(configuration, webService)
        {
        }

        #endregion

        #region Methods

        protected override IEstateFullDataItem ParseFullItem(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        protected override IEstateShortDataItem ParseShortItem(HtmlNode node)
        {
            // Nodes
            var titleNode = node.SelectSingleNode(TitlePath);
            var locationNode = node.SelectSingleNode(LocationPath);
            var priceNode = node.SelectSingleNode(PricePath);
            var linkNode = node.SelectSingleNode(LinkPath);

            // Text
            var title = titleNode?.InnerText.StripHtml();
            var location = locationNode?.InnerText.StripHtml();
            var price = priceNode?.InnerText.StripHtml();
            var link = Helper.FixLink(linkNode?.Attributes["href"].Value.StripHtml());

            var imageUrl = Helper.FixLink(ExtractImageUrl(node));

            return new ShortDataItem(imageUrl, title, location, price, link, Name);
        }

        private string ExtractImageUrl(HtmlNode parent)
        {
            var node = parent.SelectSingleNode(ImagePath);

            if (node != null && node.HasAttributes)
            {
                string[] attributes = { "data-lazy-src", "src" };

                foreach (var attribute in attributes)
                {
                    if (node.Attributes.Contains(attribute))
                        return node.Attributes[attribute].Value;
                }
            }

            return DefaultImageUrl;
        }

        #endregion

    }
}
