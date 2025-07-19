using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public  string LogoUrl { get; set; }

        public string LinkPage { get; set; }

        public bool IsBast { get; set; } = false;

    }
}
