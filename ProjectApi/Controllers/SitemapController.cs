using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("sitmap.xml")]
    public class SitemapController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSitemap()
        {
            string baseUrl = "https://api.eslamoffers.com";

            var endpoints = new List<string>
            {
                //"/api/Account/GetUsers",
                //"/api/Account/GetUser",
                //"/api/Authenticate/RefreshToken",
                "/api/Banner/GetAllBanners",
                "/api/Category/GetAllCategories",
                "/api/Coupons/GetAllCoupons",
                "/api/Coupons/GetBestCoupons/Best",
                "/api/Coupons/GetBestCoupons/BestDiscount",
                "/api/Offers/GetAllOffers",
                "/api/Offers/GetBestOffers/best",
                "/api/Store/GetAllStores",
                "/api/Store/GetBastStores/Bast",
                "/api/SubscribeEmail/GetAllEmails"
            };

            var endpointMetadata = new Dictionary<string, (string changefreq, string priority)>
            {
                //{ "/api/Account/GetUsers", ("daily", "0.6") },
                //{ "/api/Account/GetUser", ("daily", "0.6") },
                { "/api/Authenticate/RefreshToken", ("monthly", "0.4") },
                { "/api/Banner/GetAllBanners", ("weekly", "0.7") },
                { "/api/Category/GetAllCategories", ("weekly", "0.8") },
                { "/api/Coupons/GetAllCoupons", ("hourly", "0.9") },
                { "/api/Coupons/GetBestCoupons/Best", ("hourly", "0.9") },
                { "/api/Coupons/GetBestCoupons/BestDiscount", ("hourly", "0.9") },
                { "/api/Offers/GetAllOffers", ("hourly", "1.0") },
                { "/api/Offers/GetBestOffers/best", ("hourly", "1.0") },
                { "/api/Store/GetAllStores", ("weekly", "0.8") },
                { "/api/Store/GetBastStores/Bast", ("weekly", "0.8") },
                { "/api/SubscribeEmail/GetAllEmails", ("monthly", "0.5") }
            };

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var urlset = new XElement(ns + "urlset");

            foreach (var endpoint in endpoints)
            {
                var (changefreq, priority) = endpointMetadata[endpoint];

                var url = new XElement(ns + "url",
                    new XElement(ns + "loc", baseUrl + endpoint),
                    new XElement(ns + "lastmod", today),
                    new XElement(ns + "changefreq", changefreq),
                    new XElement(ns + "priority", priority)
                );

                urlset.Add(url);
            }

            var sitemap = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), urlset);

            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
