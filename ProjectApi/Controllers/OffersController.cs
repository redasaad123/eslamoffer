using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IUnitOfWork<Offers> offersUnitOfWork;

        public OffersController(IUnitOfWork<Offers> OffersUnitOfWork)
        {
            offersUnitOfWork = OffersUnitOfWork;
        }

        [HttpGet("GetAllOffers")]
        public async Task<IActionResult> GetAllOffers()
        {
            var offers = await offersUnitOfWork.Entity.GetAllAsync();
            if (offers == null || !offers.Any())
            {
                return NotFound("No offers found.");
            }
            return Ok(offers);
        }
        [HttpGet ("GetBestOffers/best")]
        public async Task<IActionResult> GetBestOffers()
        {
            var offers = await offersUnitOfWork.Entity.FindAll(x => x.IsBast == true);
            if (offers == null || !offers.Any())
            {
                return NotFound("No best offers found.");
            }
            return Ok(offers);
        }


        [HttpPost("AddOffer")]

        public async Task<IActionResult> AddOffer([FromBody] OfferDTO DTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newOffer = new Offers
            {
                Id = Guid.NewGuid().ToString(),
                Title = DTO.Title,
                LinkPage = DTO.LinkPage,
                LogoUrl = DTO.LogoUrl,
                IsBast = DTO.IsBast

            };
            await offersUnitOfWork.Entity.AddAsync(newOffer);
            offersUnitOfWork.Save();
            return Ok(newOffer);



        }
        [HttpPut("UpdateOffer/{offerId}")]
        public async Task<IActionResult> UpdateOffer([FromBody] OfferDTO DTO , string offerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var offer = await offersUnitOfWork.Entity.GetAsync(offerId);
            if (offer == null)
            {
                return NotFound("Offer not found.");
            }
            offer.Title = DTO.Title;
            offer.LinkPage = DTO.LinkPage;
            offer.LogoUrl = DTO.LogoUrl;
            offer.IsBast = DTO.IsBast;
            await offersUnitOfWork.Entity.UpdateAsync(offer);
            offersUnitOfWork.Save();
            return Ok(offer);
        }

        [HttpDelete("DeleteOffer/{offerId}")]
        public async Task<IActionResult> DeleteOffer(string offerId)
        {
            var offer = await offersUnitOfWork.Entity.GetAsync(offerId);
            if (offer == null)
            {
                return NotFound("Offer not found.");
            }
            offersUnitOfWork.Entity.Delete(offer);
            offersUnitOfWork.Save();
            return Ok("Offer deleted successfully.");



        }  
    }
}
