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

        public int? Number { get; set; } = 0;

        public DateTime? LastUseAt { get; set; }
        [NotMapped]
        public string LastUpdatedAt
        {
            get
            {
                if (!LastUseAt.HasValue) return null;

                TimeSpan difference = DateTime.Now - LastUseAt.Value ;

                // لو النتيجة سالبة (الوقت فات)
                //if (difference.TotalMilliseconds < 0)
                //{
                //    difference = DateTime.Now - LastUseAt.Value;
                //    return $"{difference.Days}:{difference.Hours}:{difference.Minutes}";
                //}
                // لو النتيجة موجبة (الوقت لسه جاي)
                return $"{difference.Days}:{difference.Hours}:{difference.Minutes}";
            }
        }
        public bool? IsActive { get; set; }

        public bool? IsBest {  get; set; }

        public bool? IsBastDiscount { get; set; }




        public string? LinkRealStore { get; set; }


        [ForeignKey("StoreId")]
        public string StoreId { get; set; }
        [NotMapped]
        public Store Store { get; set; }


    }
}
