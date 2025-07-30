using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SubscribeEmail
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public DateTime? ConfirmedAt { get; set; } = null;

    }
}
