using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;
using ProjectApi.Services;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork<Category> categoryUnitOfWork;
        private readonly StoreImage storeImage;

        public CategoryController(IUnitOfWork<Category> CategoryUnitOfWork , StoreImage storeImage)
        {
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
                Name = dto.Name,
                IconUrl = url

            };
            await categoryUnitOfWork.Entity.AddAsync(newCategory);
            categoryUnitOfWork.Save();
            return Ok(newCategory);


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




            existingCategory.Name = category.Name;
            
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



    }
}
