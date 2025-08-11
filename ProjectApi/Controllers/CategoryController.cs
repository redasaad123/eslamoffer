using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApi.DTO;
using ProjectApi.Services;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly TagsServices tagsServices;
        private readonly GenerateSlugService service;
        private readonly IUnitOfWork<Category> categoryUnitOfWork;
        private readonly StoreImage storeImage;

        public CategoryController(TagsServices tagsServices,GenerateSlugService service,IUnitOfWork<Category> CategoryUnitOfWork , StoreImage storeImage)
        {
            this.tagsServices = tagsServices;
            this.service = service;
            categoryUnitOfWork = CategoryUnitOfWork;
            this.storeImage = storeImage;
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await categoryUnitOfWork.Entity.GetAllAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }
            return Ok(categories);
        }


        [HttpGet("GetCategoryTags/{categoryId}")]
        public async Task<IActionResult> GetCategoryTags(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return BadRequest("Store ID cannot be null or empty.");
            }
            var category = categoryUnitOfWork.Entity.GetAllAsyncAsQuery().Include(x => x.CategoryTags).ThenInclude(x => x.tags).FirstOrDefault(x => x.Id == categoryId);
            if (category == null)
            {
                return NotFound("Store not found.");
            }

            var tags = category.CategoryTags.Select(t => new
            {
                t.tags.Id,
                t.tags.Name,
                t.tags.Slug,
            }).ToList();

            return Ok(tags);
        }


        [HttpPost("AddCategory")]
        [Authorize("EditorRole")]

        public async Task<IActionResult> AddCategory([FromForm] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var url = await storeImage.SaveImageAsync(dto.IconUrl);

            var newCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                createdAt = DateTime.UtcNow,
                AltText = dto.AltText ?? dto.Name,
                Slug = service.GenerateSlug(dto.Slug ?? dto.Name),
                Name = dto.Name,
                CategoryTags = new List<CategoryTags>(),
                IconUrl = url

            };

            if (dto.Tags != null)
            {
                var tags = dto.Tags.Split(',').Select(tag => tag.Trim()).ToList();
                foreach (var tag in tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                    {
                        var tagId = await tagsServices.CreateTagAsync(tag);
                        newCategory.CategoryTags.Add(new CategoryTags
                        {
                            CategoryId = newCategory.Id,
                            TagId = tagId
                        });
                    }
                }
            }
            await categoryUnitOfWork.Entity.AddAsync(newCategory);
            categoryUnitOfWork.Save();
            return Ok(newCategory);
        }

        [HttpPost]
        [Route("AddTagsToCategory/{categoryId}")]
        public async Task<IActionResult> AddTagsToCategory(string categoryId , string tags)
        {
            if (string.IsNullOrEmpty(categoryId) || string.IsNullOrEmpty(tags))
            {
                return BadRequest("Category ID and tags cannot be null or empty.");
            }
            var category = await categoryUnitOfWork.Entity.GetAsync(categoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            category.CategoryTags ??= new List<CategoryTags>();
            var tagList = tags.Split(',').Select(tag => tag.Trim()).ToList();
            foreach (var tag in tagList)
            {
                var tagId = await tagsServices.CreateTagAsync(tag);

                category.CategoryTags.Add(new CategoryTags
                {
                    CategoryId = category.Id,
                    TagId = tagId
                });
            }
            
            await categoryUnitOfWork.Entity.UpdateAsync(category);
            categoryUnitOfWork.Save();
            return Ok(category);

        }

        [HttpPut]
        [Route("UpdateCategoryTag/{categoryId}")]

        public async Task<IActionResult> UpdateCategoryTag(string categoryId, string tags)
        {
            var category = categoryUnitOfWork.Entity.GetAllAsyncAsQuery().Include(x => x.CategoryTags)
            .FirstOrDefault(x => x.Id == categoryId);
            if (category == null)
            {
                return NotFound("Store not found.");
            }

            category.CategoryTags ??= new List<CategoryTags>();
            category.CategoryTags.Clear();
            var tag = tags.Split(',').Select(t => t.Trim()).ToList();

            foreach (var newtag in tag)
            {
              
                if (!string.IsNullOrEmpty(newtag))
                {

                    var ExitingtagId = await tagsServices.CreateTagAsync(newtag);
                    
                    
                        category.CategoryTags.Add(new CategoryTags
                        {
                            CategoryId = category.Id,
                            TagId = ExitingtagId
                        });
                }
            }
            await categoryUnitOfWork.Entity.UpdateAsync(category);
            categoryUnitOfWork.Save();
            return Ok("Tag updated successfully.");

        }



        [HttpPut("UpdateCategory/{id}")]
        [Authorize("EditorRole")]
        public async Task<IActionResult> UpdateCategory(string id, [FromForm] CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingCategory = await categoryUnitOfWork.Entity.GetAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            if(category.IconUrl != null)
            {
                var url = await storeImage.SaveImageAsync(category.IconUrl);
                existingCategory.IconUrl = url;
            }



            existingCategory.Slug = service.GenerateSlug(category.Slug ?? category.Name);
            existingCategory.Name = category.Name;
            existingCategory.AltText = category.AltText ?? existingCategory.AltText;

            await categoryUnitOfWork.Entity.UpdateAsync(existingCategory);
            categoryUnitOfWork.Save();
            return Ok(existingCategory);
        }

        [HttpDelete("DeleteCategory/{id}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await categoryUnitOfWork.Entity.GetAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            categoryUnitOfWork.Entity.Delete(category);
            categoryUnitOfWork.Save();
            return Ok("Category deleted successfully.");
        }

        [HttpDelete]
        [Route("DeleteCategoryTag/{categoryId}/{tagId}")]

        public async Task<IActionResult > DeleteCategoryTag(string categoryId , string tagId)
        {

            var category = categoryUnitOfWork.Entity.GetAllAsyncAsQuery().Include(x => x.CategoryTags)
            .FirstOrDefault(x => x.Id == categoryId);
            if (category == null)
            {
                return NotFound("Store not found.");
            }
            var categoryTag = category.CategoryTags.FirstOrDefault(x => x.TagId == tagId);
            if (categoryTag == null)
            {
                return NotFound("Tag not found in the category.");
            }
            category.CategoryTags.Remove(categoryTag);
            await categoryUnitOfWork.Entity.UpdateAsync(category);
            categoryUnitOfWork.Save();
            return Ok("Tag removed from category successfully.");
        }

    }
}
