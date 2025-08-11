using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Tags
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public List<StoreTags>? StoreTags { get; set; }
        [NotMapped]
        public List<CategoryTags>? CategoryTags { get; set; }


    }
}
