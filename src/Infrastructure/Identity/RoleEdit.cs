﻿using System;
using System.Collections.Generic;
using System.Text;
 using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
namespace ComeTogether.Infrastructure.Identity
{
   

        public class RoleEdit
        {
            public IdentityRole Role { get; set; }
            public IEnumerable<ApplicationUser> Members { get; set; }
            public IEnumerable<ApplicationUser> NonMembers { get; set; }
        }
    }
