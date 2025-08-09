using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("sitemap.xml")]
    public class SitemapController : ControllerBase
    {
        private readonly AppDBContext _context;
        public SitemapController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string baseUrl = "https://api.eslamoffers.com";

            // صفحات ثابتة
            var staticPages = new List<string>
        {
            $"{baseUrl}/",
            $"{baseUrl}/about",
            $"{baseUrl}/contact"
        };

            // روابط المتاجر
            var storeUrls = _context.Stores
                .Select(s => $"{baseUrl}/store/{s.Slug}" )
                .ToList();

            // روابط التصنيفات
            var categoryUrls = _context.Categories
                .Select(c => $"{baseUrl}/category/{c.Slug}")
                .ToList();

            // روابط الكوبونات
            var couponUrls = _context.Coupons
                .Select(c => $"{baseUrl}/coupon/{c.Id}")
                .ToList();

            // روابط العروض
            var offerUrls = _context.Offers
                .Select(o => $"{baseUrl}/offer/{o.Id}")
                .ToList();

            // دمج كل الروابط
            var allUrls = staticPages
                .Concat(storeUrls)
                .Concat(categoryUrls)
                .Concat(couponUrls)
                .Concat(offerUrls)
                .Select(url => new { Url = url, LastModified = DateTime.UtcNow });

            // إنشاء XML
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XElement(ns + "urlset",
                from page in allUrls
                select new XElement(ns + "url",
                    new XElement(ns + "loc", page.Url),
                    new XElement(ns + "lastmod", page.LastModified.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "daily"),
                    new XElement(ns + "priority", "0.5")
                    

                )
            );

            return Content(new XDocument(sitemap).ToString(), "application/xml", Encoding.UTF8);
        }
    }

}
