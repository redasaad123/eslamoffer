using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork<Category> categoryUnitOfWork;

        public CategoryController(IUnitOfWork<Category> CategoryUnitOfWork)
        {
            categoryUnitOfWork = CategoryUnitOfWork;
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
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                createdAt = DateTime.UtcNow,
                Name = dto.Name,
                IconUrl = dto.IconUrl

            };
            await categoryUnitOfWork.Entity.AddAsync(newCategory);
            categoryUnitOfWork.Save();
            return Ok(newCategory);


        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingCategory = await categoryUnitOfWork.Entity.GetAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }
            existingCategory.Name = category.Name;
            existingCategory.IconUrl = category.IconUrl;
            await categoryUnitOfWork.Entity.UpdateAsync(existingCategory);
            categoryUnitOfWork.Save();
            return Ok(existingCategory);
        }

        [HttpDelete("DeleteCategory/{id}")]
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
