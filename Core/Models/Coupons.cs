using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Coupons
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string? DescriptionCoupon { get; set; }

        public string? ImageUrl { get; set; }

        public double? Discount { get; set; }

        public string? CouponCode { get; set; }

        public DateTime? StratDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? LastUseAt { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsBest {  get; set; }


        public string? LinkRealStore { get; set; }


        [ForeignKey("StoreId")]
        public string StoreId { get; set; }
        [NotMapped]
        public Store Store { get; set; }



        [ForeignKey("categoryId")]
        public string categoryId { get; set; }
        [NotMapped]
        public Category category  { get; set; }


    }
}
