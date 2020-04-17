﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ComeTogether.Infrastructure.Identity
{
   public class User
    {
       
            [Required]
            public string Name { get; set; }

            [Required]
            [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

        public byte[] Photo { get; set; }
        public string Status { get; set; }
        public string Experience { get; set; }
        public string WorkPlace { get; set; }
        public string Position { get; set; }
        public string Info { get; set; }

    }
}
