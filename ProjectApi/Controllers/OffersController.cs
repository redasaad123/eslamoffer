using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;
using ProjectApi.Services;
using System;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly StoreImage storeImage;
        private readonly IUnitOfWork<Offers> offersUnitOfWork;

        public OffersController(StoreImage storeImage,IUnitOfWork<Offers> OffersUnitOfWork)
        {
            this.storeImage = storeImage;
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

        public async Task<IActionResult> AddOffer([FromForm] OfferDTO DTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var urlLogo = await storeImage.SaveImageAsync(DTO.LogoUrl);

            var urlImage = await storeImage.SaveImageAsync(DTO.ImageStoreUrl);






            var newOffer = new Offers
            {
                Id = Guid.NewGuid().ToString(),
                Title = DTO.Title,
                couponId = DTO.couponId,
                Price = DTO.Price,
                CurrencyCodes = DTO.CurrencyCodes,
                Discount = DTO.Discount,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
                LinkPage = DTO.LinkPage,
                ImageStoreUrl = urlImage,
                LogoUrl = urlLogo,
                IsBast = DTO.IsBast

            };
            await offersUnitOfWork.Entity.AddAsync(newOffer);
            offersUnitOfWork.Save();
            return Ok(newOffer);



        }
        [HttpPut("UpdateOffer/{offerId}")]
        public async Task<IActionResult> UpdateOffer([FromForm] OfferDTO DTO , string offerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var offer = await offersUnitOfWork.Entity.GetAsync(offerId);
            if (offer == null)
            {
                return NotFound("Offer not found.");
            }


            if (DTO.LogoUrl != null && DTO.LogoUrl.Length > 0)
            {
                var urlLogo = await storeImage.SaveImageAsync(DTO.LogoUrl);


                offer.LogoUrl = urlLogo;
            }

            if (DTO.ImageStoreUrl != null && DTO.ImageStoreUrl.Length > 0)
            {
                var urlImage = await storeImage.SaveImageAsync(DTO.ImageStoreUrl);
                offer.ImageStoreUrl = urlImage;
            }

            offer.CurrencyCodes = DTO.CurrencyCodes;
            offer.Title = DTO.Title;
            offer.Price = DTO.Price;
            offer.Discount = DTO.Discount;
            offer.LastUpdatedAt = DateTime.Now;
            offer.LinkPage = DTO.LinkPage;
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
