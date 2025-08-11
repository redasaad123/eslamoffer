using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class StoreTags
    {
        public string StoreId { get; set; }

        [NotMapped]
        public Store? Store { get; set; }

        public string TagId { get; set; }
        [NotMapped]
        public Tags? Tag { get; set; }
    }
}
