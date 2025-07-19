namespace ProjectApi.DTO
{
    public class OfferDTO
    {
        public string Title { get; set; }
        public string LogoUrl { get; set; }
        public string LinkPage { get; set; }

        public bool IsBast { get; set; } = false;
    }
}
