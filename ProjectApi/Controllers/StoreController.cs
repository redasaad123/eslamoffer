using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IUnitOfWork<Store> storeUnitOfWork;

        public StoreController(IUnitOfWork<Store> StoreUnitOfWork)
        {
            storeUnitOfWork = StoreUnitOfWork;
        }


        [HttpGet("GetAllStores")]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await storeUnitOfWork.Entity.GetAllAsync();
            if (stores == null || !stores.Any())
            {
                return NotFound("No stores found.");
            }
            return Ok(stores);
        }

        [HttpGet("GetBastStores/Bast")]
        public async Task<IActionResult> GetBastStores()
        {
            var stores = await storeUnitOfWork.Entity.FindAll(x=>x.IsBast == true);
            if (stores == null || !stores.Any())
            {
                return NotFound("No stores found.");
            }
            return Ok(stores);
        }

        [HttpGet("GetStoreById/{id}")]
        public async Task<IActionResult> GetStoreById(string id)
        {
            var store = await storeUnitOfWork.Entity.GetAsync(id);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            var stores = new
            {
                Id = store.Id,
                Name = store.Name,
                LogoUrl = store.LogoUrl,
                CreatedAt = store.CreatedAt,
                LastUpdatedAt = store.LastUpdatedAt,
                HeaderDescription = store.HeaderDescription,
                Description = store.Description,
            };

            return Ok(store);
        }

        [HttpPost("AddStore")]
        public async Task<IActionResult> AddStore([FromForm] StoreDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads");


                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // استخدم اسم الملف الأصلي (بعد تنظيفه)
                var fileName = Path.GetFileName(dto.ImageUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                //imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            }

            var store = new Store
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Isactive = true,
                LastUpdatedAt = DateTime.UtcNow,
                HeaderDescription = dto.HeaderDescription,
                Description = dto.Description,
                Name = dto.Name,
                LogoUrl = dto.ImageUrl.FileName,
                IsBast = dto.IsBast
            };
            var addedStore = await storeUnitOfWork.Entity.AddAsync(store);
            storeUnitOfWork.Save();
            return Ok(store);
        }

        [HttpPut("UpdateStore/{id}")]
        public async Task<IActionResult> UpdateStore(string id, [FromForm] StoreDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var store = await storeUnitOfWork.Entity.GetAsync(id);
            if (store == null)
            {
                return NotFound("Store not found.");
            }

            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads");


                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // استخدم اسم الملف الأصلي (بعد تنظيفه)
                var fileName = Path.GetFileName(dto.ImageUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);

                }
                store.LogoUrl = dto.ImageUrl.FileName;

                //imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            }

            store.Name = dto.Name;
            store.HeaderDescription = dto.HeaderDescription;
            store.Description = dto.Description;
            store.IsBast = dto.IsBast;
            store.LastUpdatedAt = DateTime.UtcNow;
            var updatedStore = await storeUnitOfWork.Entity.UpdateAsync(store);
            storeUnitOfWork.Save();
            return Ok(updatedStore);
        }


        [HttpDelete("DeleteStore/{id}")]
        public async Task<IActionResult> DeleteStore(string id)
        {
            var store = await storeUnitOfWork.Entity.GetAsync(id);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            storeUnitOfWork.Entity.Delete(store);
            storeUnitOfWork.Save();
            return Ok("Store deleted successfully.");


        }
    }
}
