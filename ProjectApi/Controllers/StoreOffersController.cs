using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;
using ProjectApi.Services;
using System.Threading.Tasks;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreOffersController : ControllerBase
    {
        private readonly IUnitOfWork<StoresOffers> storesOffersUnitOfWork;
        private readonly StoreImage storeImage;

        public StoreOffersController(IUnitOfWork<StoresOffers> storesOffersUnitOfWork , StoreImage storeImage )
        {
            this.storesOffersUnitOfWork = storesOffersUnitOfWork;
            this.storeImage = storeImage;
        }

        [HttpGet("GetAllOffers")]
        public async Task<IActionResult> GetAllOffers()
        {
            var offers =await storesOffersUnitOfWork.Entity.GetAllAsync();
            if (offers == null || !offers.Any())
            {
                return NotFound("No offers found.");
            }
            return Ok(offers);
        }

        [HttpPost("AddOffer")]
        [Authorize("EditorRole")]
        public async Task<IActionResult> CreateOffer([FromForm] StoreOfferDTO dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var imageUrl = await storeImage.SaveImageAsync(dto.LogoUrl);


            var offer = new StoresOffers
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Description = dto.Description,
                LogoUrl = imageUrl ,
                AltText = dto.AltText,
                LinkPage = dto.LinkPage,
                SlugStore = dto.SlugStore,
                CreatedAt = DateTime.Now
            };

            await storesOffersUnitOfWork.Entity.AddAsync(offer);
            storesOffersUnitOfWork.Save();
            return Ok(offer);
        }

        [HttpPut("UpdateOffer/{id}")]
        [Authorize("EditorRole")]
        public async Task<IActionResult> UpdateOffer(string id, [FromForm] StoreOfferDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingOffer = await storesOffersUnitOfWork.Entity.GetAsync(id);
            if (existingOffer == null)
            {
                return NotFound($"Offer with ID {id} not found.");
            }

            if (dto.LogoUrl != null)
            {
                var imageUrl = await storeImage.SaveImageAsync(dto.LogoUrl);
                existingOffer.LogoUrl = imageUrl;
            }
            existingOffer.Title = dto.Title;
            existingOffer.Description = dto.Description;
            existingOffer.AltText = dto.AltText;
            existingOffer.LinkPage = dto.LinkPage;
            existingOffer.SlugStore = dto.SlugStore;
            await storesOffersUnitOfWork.Entity.UpdateAsync(existingOffer);
            storesOffersUnitOfWork.Save();
            return Ok(existingOffer);
        }

        [HttpDelete("DeleteOffer/{id}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> DeleteOffer(string id)
        {
            var existingOffer = await storesOffersUnitOfWork.Entity.GetAsync(id);
            if (existingOffer == null)
            {
                return NotFound($"Offer with ID {id} not found.");
            }
            storesOffersUnitOfWork.Entity.Delete(existingOffer);
            storesOffersUnitOfWork.Save();
            return Ok($"Offer with ID {id} deleted successfully.");
        }

    }
}
