﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AuthenticationDTO
{
    public class RegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }

    }
}
