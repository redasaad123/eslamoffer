using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Store
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
        public string? HeaderDescription { get; set; }


        public bool? Isactive { get; set; }

        public bool IsBast {  get; set; }

        public List<string>? Categorys { get; set; } = new List<string>();

        public List<DescriptionStore>? DescriptionStore { get; set; } 

    }
}
