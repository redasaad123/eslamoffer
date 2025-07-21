namespace ProjectApi.DTO
{
    public class OfferDTO
    {
        public string Title { get; set; }
        public IFormFile LogoUrl { get; set; }
        public string LinkPage { get; set; }

        public bool IsBast { get; set; } = false;
    }
}
