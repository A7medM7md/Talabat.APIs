using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }

        public Address Address { get; set; } // Nav Prop [ONE]
    }
}
/*
 # SQL Server Doesn't Know Take Which PK and Set It As FK On Which Entity, So I Make It Manually
    Principle Entity Is AppUser So I Take Id For AppUser, Set It As FK On Address
*/