using System;
using EstateParser.Contracts;
using EstateParser.Contracts.Data;
using EstateParser.Contracts.Services;
using EstateParser.Core.Models;
using EstateParser.Core.Tools;
using HtmlAgilityPack;

namespace EstateParser.Core.Providers
{
    public class IrrRuProvider : WebBaseProvider
    {
        #region Properties

        public override string Name { get; } = "Irr.Ru";

        public override int MaxOffset { get; } = 1000;

        protected override int ItemsPerPage { get; set; } = 30;

        protected override string BaseUrl { get; } = "https://russia.irr.ru";

        protected override string RequestUrl { get; } = "https://russia.irr.ru/real-estate/apartments-sale/page{0}/";

        protected override string ItemsClass =>
            "//div[contains(@class,\'js-productBlock\')]";

        private string ImagePath =>
            string.Format(PathTemplate, "img", "listing__image js-lazy");

        private string PricePath =>
            string.Format(PathTemplate, "div", "listing__itemPrice");

        private string LocationPath =>
            string.Format(PathTemplate + "//span[@class=\"listing__itemPlace\"]", "div", "listing__itemDescription");

        private string LinkPath =>
            string.Format(PathTemplate, "a", "listing__itemTitle");

        #endregion

        #region Constructor

        public IrrRuProvider(IConfiguration configuration, IWebService webService) : base(configuration, webService)
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
            var imageNode = node.SelectSingleNode(ImagePath);
            var locationNode = node.SelectNodes(LocationPath)[1];
            var priceNode = node.SelectSingleNode(PricePath);
            var linkNode = node.SelectSingleNode(LinkPath);

            // Text
            var title = imageNode?.Attributes["alt"].Value.StripHtml();
            var location = locationNode?.InnerText.StripHtml();
            var price = priceNode?.InnerText.StripHtml();

            var link = Helper.FixLink(linkNode?.Attributes["href"].Value);
            var imageUrl = Helper.FixLink(ExtractImageUrl(node));

            return new ShortDataItem(imageUrl, title, location, price, link, Name);
        }

        private string ExtractImageUrl(HtmlNode parent)
        {
            var node = parent.SelectSingleNode(ImagePath);

            if (node != null && node.HasAttributes)
            {
                return node.Attributes["data-src"].Value;
            }

            return DefaultImageUrl;
        }

        #endregion
    }
}
