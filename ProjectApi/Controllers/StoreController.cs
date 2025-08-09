using Core.Interfaces;
using Core.Models;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
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
        private readonly GenerateSlugService slugservices;
        private readonly StoreImage services;

        public StoreController(IUnitOfWork<Store> StoreUnitOfWork , StoreImage services , GenerateSlugService Slugservices)
        {
            storeUnitOfWork = StoreUnitOfWork;
            slugservices = Slugservices;
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

        [HttpGet("GetStoreBySlug/{slug}")]
        public async Task<IActionResult> GetStoreById(string slug)
        {
            var store = storeUnitOfWork.Entity.Find(x=> x.Slug== slug);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            //var stores = new
            //{
            //    Id = store.Id,
            //    Name = store.Name,
            //    LogoUrl = store.LogoUrl,
            //    CreatedAt = store.CreatedAt,
            //    LastUpdatedAt = store.LastUpdatedAt,
            //    HeaderDescription = store.HeaderDescription,
            //    Description = store.Description,
            //};

            return Ok(store);
        }

        [HttpGet("GetStoresByCategory/{slug}")]
        public async Task<IActionResult> GetStoresByCategory(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest("Category ID cannot be null or empty.");
            }
            var stores = await storeUnitOfWork.Entity.GetAllAsync();
            if (stores == null || !stores.Any())
            {
                return NotFound("No stores found for the specified category.");
            }

            var Filter = stores.Where(x => x.Categorys.Contains(slug)).ToList();

            return Ok(Filter);
        }

        [HttpPost("AddStore")]
        [Authorize("EditorRole")]
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
                AltText = dto.AltText,
                Name = dto.Name,
                Slug = slugservices.GenerateSlug(dto.Slug ?? dto.Name),
                LogoUrl = url,
                IsBast = dto.IsBast,
                Categorys = new List<string>(),
                DescriptionStore = new List<DescriptionStore>()

            };
            if (dto.SlugCategory != null || dto.SlugCategory.Any())
            {
                store.Categorys.AddRange(dto.SlugCategory);
            }

            foreach (var ds in dto.descriptionStores)
            {
                var descriptionStore = new DescriptionStore
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = ds.Description,
                    SubHeader = ds.SubHeader,
                    Image = ds.Image != null
                        ? await services.SaveImageAsync(ds.Image)
                        : null,
                    AltText = ds.AltText,
                    
                };

                store.DescriptionStore.Add(descriptionStore);
            }


            var addedStore = await storeUnitOfWork.Entity.AddAsync(store);
            storeUnitOfWork.Save();
            return Ok(store);
        }

        [HttpPut("UpdateStore/{id}")]
        [Authorize("EditorRole")]
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

            if(dto.IsUpdateCategory == true)
            {
                store.Categorys = new List<string>();
                if (dto.SlugCategory != null || dto.SlugCategory.Any())
                {
                    store.Categorys.AddRange(dto.SlugCategory);
                }
            }

            store.Name = dto.Name;
            store.HeaderDescription = dto.HeaderDescription;
            store.Description = dto.Description;
            store.Slug = slugservices.GenerateSlug(dto.Slug ?? dto.Name);
            store.IsBast = dto.IsBast;
            store.AltText = dto.AltText;
            store.LastUpdatedAt = DateTime.UtcNow;
            var updatedStore = await storeUnitOfWork.Entity.UpdateAsync(store);
            storeUnitOfWork.Save();
            return Ok(updatedStore);
        }

        [HttpPost("AddDescriptionStore/{storeId}")]
        public async Task<IActionResult> AddDescriptionStore(DescriptionStoreDTO DTO , string storeId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var store = await storeUnitOfWork.Entity.GetAsync(storeId);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            var descriptionStore = new DescriptionStore
            {
                Id = Guid.NewGuid().ToString(),
                Description = DTO.Description,
                SubHeader = DTO.SubHeader,
                Image = DTO.Image != null ? await services.SaveImageAsync(DTO.Image) : null,
                AltText = DTO.AltText
            };
            store.DescriptionStore.Add(descriptionStore);
            await storeUnitOfWork.Entity.UpdateAsync(store);
            storeUnitOfWork.Save();
            return Ok(descriptionStore);

        }




        [HttpPut("UpdateDescriptionStore/{storeId}/{Id}")]
        public async Task<IActionResult> UpdateDescriptionStore(string Id , string storeId ,[FromForm] DescriptionStoreDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var store = await storeUnitOfWork.Entity.GetAsync(storeId);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            var descriptionStore = store.DescriptionStore.FirstOrDefault(x => x.Id == Id);
            if (descriptionStore == null)
            {
                return NotFound("Description not found.");
            }
            if (dTO.Image != null)
            {
                descriptionStore.Image = await services.SaveImageAsync(dTO.Image);
            }
            descriptionStore.SubHeader = dTO.SubHeader;
            descriptionStore.AltText = dTO.AltText;
            descriptionStore.Description = dTO.Description;
            await storeUnitOfWork.Entity.UpdateAsync(store);
            storeUnitOfWork.Save();
            return Ok(descriptionStore);


        }


        [HttpDelete("DeleteStore/{id}")]
        [Authorize("AdminRole")]
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

        [HttpDelete("DeleteDescriptionStore/{storeId}/{Id}")]
        public async Task<IActionResult> DeleteDescriptionStore(string Id, string storeId)
        {
            var store = await storeUnitOfWork.Entity.GetAsync(storeId);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            var descriptionStore = store.DescriptionStore.FirstOrDefault(x => x.Id == Id);
            if (descriptionStore == null)
            {
                return NotFound("Description not found.");
            }
            store.DescriptionStore.Remove(descriptionStore);
            await storeUnitOfWork.Entity.UpdateAsync(store);
            storeUnitOfWork.Save();
            return Ok("Description deleted successfully.");
        }

    }
}
