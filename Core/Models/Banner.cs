using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Banner
    {
        public string Id { get; set; }
        public string? ImageUrl { get; set; }

        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? Priority { get; set; } 

    }
}
