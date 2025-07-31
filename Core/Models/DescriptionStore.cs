using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    [Owned]
    public class DescriptionStore
    {
        public string Id { get; set; }

        public string? SubHeader { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }
    }
}
