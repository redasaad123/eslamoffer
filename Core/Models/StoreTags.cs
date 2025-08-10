using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class StoreTags
    {
        public string StoreId { get; set; }
        public Store? Store { get; set; }

        public string TagId { get; set; }

        public Tags? Tag { get; set; }
    }
}
