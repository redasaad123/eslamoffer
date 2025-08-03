namespace ProjectApi.DTO
{
    public class CategoryDTO
    {

        public string Name { get; set; }
        public string? Slug { get; set; }

        public IFormFile? IconUrl { get; set; }
    }
}
