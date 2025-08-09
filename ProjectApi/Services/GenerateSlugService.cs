using Core.Interfaces;

namespace ProjectApi.Services
{
    public class GenerateSlugService 
    {
        public string GenerateSlug(string title )
        {
            return title.ToLower().Replace(" ", "-").Replace(":", "").Replace(",", "");
        }

    }
}
