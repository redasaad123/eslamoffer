using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;
using ProjectApi.Services;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IUnitOfWork<Store> storeUnitOfWork;
        private readonly StoreImage services;

        public StoreController(IUnitOfWork<Store> StoreUnitOfWork , StoreImage services)
        {
            storeUnitOfWork = StoreUnitOfWork;
            this.services = services;
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

            var url = await services.SaveImageAsync(dto.ImageUrl);

            var store = new Store
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Isactive = true,
                LastUpdatedAt = DateTime.UtcNow,
                HeaderDescription = dto.HeaderDescription,
                Description = dto.Description,
                Name = dto.Name,
                LogoUrl = url,
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

            if(dto.ImageUrl != null)
            {
                var url = await services.SaveImageAsync(dto.ImageUrl);
                store.LogoUrl = url;
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
