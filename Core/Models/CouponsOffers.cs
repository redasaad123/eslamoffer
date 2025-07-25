using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CouponsOffers
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? CouponCode { get; set; }
        public DateTime? StratDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }


    }
}
