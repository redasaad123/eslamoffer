namespace ProjectApi.DTO
{
    public class OfferDTO
    {
        public string Title { get; set; }
        public IFormFile? LogoUrl { get; set; }

        public string? AltText { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public string? LinkPage { get; set; }
        public string? couponId { get; set; }

        public string? CurrencyCodes { get; set; }

        public IFormFile? ImageStoreUrl { get; set; }

        public bool IsBast { get; set; } = false;
    }
}
