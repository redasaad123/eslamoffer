using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public string? Photo {  get; set; }

        public string? Address { get; set; }


        [Column(TypeName = "varchar(191)")]
        public override string NormalizedUserName { get; set; }

        [Column(TypeName = "varchar(191)")]
        public override string NormalizedEmail { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
