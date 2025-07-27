namespace ProjectApi.DTO
{
    public class BannerDTO
    {
        public IFormFile? ImageUrl { get; set; }

        public string? Link { get; set; }

        public int? Priority { get; set; }=0;

    }
}
