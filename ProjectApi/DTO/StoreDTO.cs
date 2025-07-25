namespace ProjectApi.DTO
{
    public class StoreDTO
    {

        public string Name { get; set; }

        public IFormFile? ImageUrl { get; set; }
        public string? HeaderDescription { get; set; }

        public string? Description { get; set; }

        public bool IsBast { get; set; }
    }
}
