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

        public async Task<IActionResult> AddOffer([FromForm] OfferDTO DTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (DTO.LogoUrl != null && DTO.LogoUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads");


                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // استخدم اسم الملف الأصلي (بعد تنظيفه)
                var fileName = Path.GetFileName(DTO.LogoUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await DTO.LogoUrl.CopyToAsync(stream);
                }

                //imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            }





            var newOffer = new Offers
            {
                Id = Guid.NewGuid().ToString(),
                Title = DTO.Title,
                LinkPage = DTO.LinkPage,
                LogoUrl = DTO.LogoUrl.FileName,
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
            offer.Title = DTO.Title;
            offer.LinkPage = DTO.LinkPage;
            offer.LogoUrl = DTO.LogoUrl.FileName;
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
