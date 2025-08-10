using Core.Interfaces;
using Core.Models;

namespace ProjectApi.Services
{
    public class TagsServices
    {
        private readonly GenerateSlugService generateSlug;
        private readonly IUnitOfWork<Tags> tagsUnitOfWork;

        public TagsServices(GenerateSlugService generateSlug , IUnitOfWork<Tags> TagsUnitOfWork)
        {
            this.generateSlug = generateSlug;
            tagsUnitOfWork = TagsUnitOfWork;
        }


        public async Task<string> CreateTagAsync(string tags)
        {
            var tag = new Tags
            {
                Id = Guid.NewGuid().ToString(),
                Name = tags,
                Slug = generateSlug.GenerateSlug(tags),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                StoreTags = new List<StoreTags>()
            };
            await tagsUnitOfWork.Entity.AddAsync(tag);
            tagsUnitOfWork.Save();

            return tag.Id;

        }
    }
}
