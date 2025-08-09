using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser> /*Inherits 7 DbSets For Identity*/
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options):base(options) /*Chain Directly On Ctor That Takes Options*/
        {
        }

    }
}
