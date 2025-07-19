using Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectApi.DTO
{
    public class CouponsDTO
    {

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public double? Discount { get; set; }

        public string? CouponCode { get; set; }

        public DateTime? StratDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsBest { get; set; }

        public string? LinkRealStore { get; set; }

        public string StoreId { get; set; }

        public string categoryId { get; set; }


    }
}
