namespace ProjectApi.DTO
{
    public class StoreOfferDTO
    {

        public string? Title { get; set; }

        public string? Description { get; set; }
        public IFormFile? LogoUrl { get; set; }

        public string? AltText { get; set; }

        public string? LinkPage { get; set; }

        public bool? IsBest { get; set; } = false;

        public bool? IsBastDiscount { get; set; } = false;

        public string? SlugStore { get; set; }

    }
}
