using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CategoryTags
    {
        public string CategoryId { get; set; }

        public Category? Category { get; set; }


        public string TagId { get; set; }

        public Tags? tags { get; set; }

    }
}
