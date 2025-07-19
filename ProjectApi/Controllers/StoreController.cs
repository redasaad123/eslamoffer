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

        [HttpPost("AddStore")]
        public async Task<IActionResult> AddStore([FromBody] StoreDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var store = new Store
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Isactive = true,
                LastUpdatedAt = DateTime.UtcNow,
                Name = dto.Name,
                LogoUrl = dto.LogoUrl,
                IsBast = dto.IsBast
            };
            var addedStore = await storeUnitOfWork.Entity.AddAsync(store);
            storeUnitOfWork.Save();
            return Ok(store);
        }

        [HttpPut("UpdateStore/{id}")]
        public async Task<IActionResult> UpdateStore(string id, [FromBody] StoreDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var store = await storeUnitOfWork.Entity.GetAsync(id);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            store.Name = dto.Name;
            store.LogoUrl = dto.LogoUrl;
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
