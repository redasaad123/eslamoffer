using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Offers
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public  string? LogoUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public string? LinkPage { get; set; }
        public bool IsBast { get; set; } = false;

        [ForeignKey("storeId")]
        public string? storeId { get; set; }
        [NotMapped]
        public Store? store { get; set; }

        [ForeignKey("couponId")]
        public string? couponId { get; set; }

        [NotMapped]
        public CouponsOffers? coupon { get; set; }




    }
}
