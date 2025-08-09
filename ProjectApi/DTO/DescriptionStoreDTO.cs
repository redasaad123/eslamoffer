namespace ProjectApi.DTO
{
    public class DescriptionStoreDTO
    {
        public string? SubHeader { get; set; }

        public string? Description { get; set; }

        public IFormFile? Image { get; set; }

        public string? AltText { get; set; }
    }
}
