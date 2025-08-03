using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Category
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string? IconUrl { get; set; }

        public string? Slug { get; set; }
        public DateTime createdAt { get; set; }

        
    }
}
