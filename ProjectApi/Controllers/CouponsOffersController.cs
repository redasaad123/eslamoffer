using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsOffersController : ControllerBase
    {
        private readonly IUnitOfWork<CouponsOffers> couponsOffersUnitOfWork;

        public CouponsOffersController(IUnitOfWork<CouponsOffers> CouponsOffersUnitOfWork)
        {
            couponsOffersUnitOfWork = CouponsOffersUnitOfWork;
        }

        [HttpGet("GetCouponsOffers/{Id}")]
        public async Task<IActionResult> GetCouponsOffers(string Id)
        {
            var couponOffer = await couponsOffersUnitOfWork.Entity.GetAsync(Id);
            if (couponOffer == null)
            {
                return NotFound("Coupon offer not found.");
            }
            return Ok(couponOffer);
        }

        [HttpPost]
        [Authorize("AdminRole")]
        public async Task<IActionResult> AddCouponOffer([FromBody] CoupnsOffersDTO DTO )
        {
            if (!ModelState.IsValid)
                 return BadRequest(ModelState);

            var couponOffer = new CouponsOffers
            {
                Id = Guid.NewGuid().ToString(),
                Title = DTO.Title,
                CouponCode = DTO.CouponCode,
                StratDate = DateTime.Now,
                EndDate = DTO.EndDate,
                CreatedAt = DateTime.Now
            };
            await couponsOffersUnitOfWork.Entity.AddAsync(couponOffer);
            couponsOffersUnitOfWork.Save();
            return Ok();
        }

        [HttpPut("UpdateCouponOffer/{Id}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> UpdateCouponOffer(string Id, [FromBody] CoupnsOffersDTO DTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var couponOffer = await couponsOffersUnitOfWork.Entity.GetAsync(Id);
            if (couponOffer == null)
            {
                return NotFound("Coupon offer not found.");
            }
            couponOffer.Title = DTO.Title;
            couponOffer.CouponCode = DTO.CouponCode;
            couponOffer.EndDate = DTO.EndDate;
            await couponsOffersUnitOfWork.Entity.UpdateAsync(couponOffer);
            couponsOffersUnitOfWork.Save();
            return Ok(couponOffer);
        }


        [HttpDelete("DeleteCouponOffer/{Id}")]
        [Authorize("AdminRole")]
        public async Task< IActionResult> DeleteCouponOffer(string Id)
        {
            var couponOffer = await couponsOffersUnitOfWork.Entity.GetAsync(Id);
            if (couponOffer == null)
            {
                return NotFound("Coupon offer not found.");
            }

            couponsOffersUnitOfWork.Entity.Delete(couponOffer);
            couponsOffersUnitOfWork.Save();
            return Ok(couponOffer);


        }

    }
}
