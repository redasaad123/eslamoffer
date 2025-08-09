using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;
using ProjectApi.Services;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BannerController : ControllerBase
    {
        private readonly IUnitOfWork<Banner> unitOfWork;
        private readonly StoreImage storeImage;

        public BannerController(IUnitOfWork<Banner> unitOfWork , StoreImage storeImage)
        {
            this.unitOfWork = unitOfWork;
            this.storeImage = storeImage;
        }

        [HttpGet ("GetAllBanners")]
        public async Task<IActionResult> GetAllBanners()
        {
            var banners = await unitOfWork.Entity.GetAllAsync();

            if (banners == null || !banners.Any())
            {
                return NotFound("No banners found.");
            }
            var sorting = banners.OrderByDescending(x => x.Priority).ThenByDescending(x => x.CreatedAt).ToList();


            return Ok(sorting);
        }

        [HttpPost("AddBanner")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> AddBanner([FromForm] BannerDTO DTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageUrl = await storeImage.SaveImageAsync(DTO.ImageUrl);

            var banner = new Banner
            {
                Id = Guid.NewGuid().ToString(),
                ImageUrl = imageUrl,
                AltText = DTO.AltText,
                Link = DTO.Link,
                Priority = DTO.Priority 
            };
            
            await unitOfWork.Entity.AddAsync(banner);
            unitOfWork.Save();
            return Ok(banner);
        }

        [HttpPut("UpdateBanner/{Id}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> UpdateBanner([FromForm] BannerDTO DTO , string Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var banner = await unitOfWork.Entity.GetAsync(Id);
            if (banner == null)
            {
                return NotFound("Banner not found.");
            }
            if (DTO.ImageUrl != null)
            {
                banner.ImageUrl = await storeImage.SaveImageAsync(DTO.ImageUrl);
            }
            banner.Link = DTO.Link;
            banner.Priority = DTO.Priority;
            banner.AltText = DTO.AltText;
            await unitOfWork.Entity.UpdateAsync(banner);
            unitOfWork.Save();
            return Ok(banner);
        }

        [HttpDelete("DeleteBanner/{Id}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> DeleteBanner(string Id)
        {
            var banner = await unitOfWork.Entity.GetAsync(Id);
            if (banner == null)
            {
                return NotFound("Banner not found.");
            }
            unitOfWork.Entity.Delete(banner);
            unitOfWork.Save();
            return Ok("Banner deleted successfully.");

        }


    }
}
