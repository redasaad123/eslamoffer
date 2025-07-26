using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;
using ProjectApi.Services;
using SixLabors.ImageSharp;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly StoreImage storeImage;
        private readonly IUnitOfWork<Store> storeUnitOfWork;
        private readonly IUnitOfWork<Coupons> couponsUnitOfWork;
        private readonly IUnitOfWork<Category> categoryUnitOfWork;

        public CouponsController( StoreImage storeImage,IUnitOfWork<Store> StoreUnitOfWork , IUnitOfWork<Coupons> CouponsUnitOfWork, IUnitOfWork<Category> CategoryUnitOfWork)
        {
            this.storeImage = storeImage;
            storeUnitOfWork = StoreUnitOfWork;
            couponsUnitOfWork = CouponsUnitOfWork;
            categoryUnitOfWork = CategoryUnitOfWork;
        }


        [HttpGet("GetAllCoupons")]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await couponsUnitOfWork.Entity.GetAllAsync();
            if (coupons == null || !coupons.Any())
            {
                return NotFound("No coupons found.");
            }
            return Ok(coupons);
        }


        [HttpGet("GetBestCoupons/Best")]
        public async Task<IActionResult> GetBestCoupons()
        {
            var coupons = await couponsUnitOfWork.Entity.FindAll(x => x.IsBest == true);
            if (coupons == null || !coupons.Any())
            {
                return NotFound("No best Coupons found.");
            }
            return Ok(coupons);
        }



        [HttpGet("GetCouponsInCategory/{catrgoryId}")]
        public async Task<IActionResult> GetCouponsInCategory(string catrgoryId)
        {
            var Coupons = await couponsUnitOfWork.Entity.FindAll(x => x.categoryId == catrgoryId);
            if (Coupons == null || !Coupons.Any())
            {
                return NotFound("No Coupons found in this category.");
            }
            return Ok(Coupons);

        }

        [HttpGet]
        [Route("GetCouponsByStore/{storeId}")]
        public async Task<IActionResult> GetCouponsByStore(string storeId)
        {
            var coupons = await couponsUnitOfWork.Entity.FindAll(x => x.StoreId == storeId);
            if (coupons == null || !coupons.Any())
            {
                return NotFound("No coupons found for this store.");
            }

            return Ok(coupons);
        }


        [HttpPost("AddCoupon")]
        public async Task<IActionResult> AddCoupon([FromForm] CouponsDTO DTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var store = await storeUnitOfWork.Entity.GetAsync(DTO.StoreId);
            if (store == null)
            {
                return NotFound("Store not found.");
            }
            var category = await categoryUnitOfWork.Entity.GetAsync(DTO.categoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var url = await storeImage.SaveImageAsync(DTO.ImageUrl);
            var Coupons = new Coupons
            {
                Id = Guid.NewGuid().ToString(),
                Title = DTO.Title,
                DescriptionCoupon = DTO.Description,
                ImageUrl = url,
                Discount = DTO.Discount,
                CouponCode = DTO.CouponCode,
                StratDate = DTO.StratDate,
                EndDate = DTO.EndDate,
                CreatedAt = DateTime.Now,
                LastUseAt =  DateTime.UtcNow,
                IsActive = DTO.IsActive ?? true,
                IsBest = DTO.IsBest ?? false,
                LinkRealStore = DTO.LinkRealStore,
                StoreId = store.Id,
                categoryId = category.Id
            };
            await couponsUnitOfWork.Entity.AddAsync(Coupons);
            couponsUnitOfWork.Save();
            return Ok(Coupons);

        }

        [HttpPut("NumberUsed/{Id}")]
        public async Task<IActionResult> NumberUsed(string Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Coupons = await couponsUnitOfWork.Entity.GetAsync(Id);
            if (Coupons == null)
            {
                return NotFound("Coupons not found.");
            }
            Coupons.LastUseAt = DateTime.UtcNow;
            Coupons.Number = Coupons.Number + 1;
            await couponsUnitOfWork.Entity.UpdateAsync(Coupons);
            couponsUnitOfWork.Save();
            return Ok(new { LastUseAt = Coupons.LastUseAt , Number = Coupons.Number });
        }


        [HttpPut("UpdateCoupon/{id}")]
        public async Task<IActionResult> UpdateOffer(string id ,[FromForm] CouponsDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Coupons = await couponsUnitOfWork.Entity.GetAsync(id);
            if (Coupons == null)
            {
                return NotFound("Coupons not found.");
            }

            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var url = await storeImage.SaveImageAsync(dto.ImageUrl);
                Coupons.ImageUrl = url;
            }

            Coupons.Title = dto.Title;
            Coupons.DescriptionCoupon = dto.Description;
            Coupons.Discount = dto.Discount;
            Coupons.CouponCode = dto.CouponCode;
            Coupons.StratDate = dto.StratDate;
            Coupons.EndDate = dto.EndDate;
            Coupons.IsActive = dto.IsActive ?? true;
            Coupons.IsBest = dto.IsBest ?? false;
            Coupons.LinkRealStore = dto.LinkRealStore;
            await couponsUnitOfWork.Entity.UpdateAsync(Coupons);
            couponsUnitOfWork.Save();
            return Ok(Coupons);

        }

        [HttpDelete("DeleteCoupons/{id}")]
        public async Task<IActionResult> DeleteCoupons(string id)
        {
            var Coupons = await couponsUnitOfWork.Entity.GetAsync(id);
            if (Coupons == null)
            {
                return NotFound("Coupons not found.");
            }
            couponsUnitOfWork.Entity.Delete(Coupons);
            couponsUnitOfWork.Save();
            return Ok("Coupons deleted successfully.");

        }

    }
}
