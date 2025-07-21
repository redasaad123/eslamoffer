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
        public async Task<IActionResult> AddCategory([FromForm] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (dto.IconUrl != null && dto.IconUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads");


                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // استخدم اسم الملف الأصلي (بعد تنظيفه)
                var fileName = Path.GetFileName(dto.IconUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.IconUrl.CopyToAsync(stream);
                }

                //imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            }




            var newCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                createdAt = DateTime.UtcNow,
                Name = dto.Name,
                IconUrl = dto.IconUrl.FileName

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

            if (category.IconUrl != null && category.IconUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads");


                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // استخدم اسم الملف الأصلي (بعد تنظيفه)
                var fileName = Path.GetFileName(category.IconUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await category.IconUrl.CopyToAsync(stream);
                }

                //imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                existingCategory.IconUrl = category.IconUrl.FileName;
            }



            existingCategory.Name = category.Name;
            
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
