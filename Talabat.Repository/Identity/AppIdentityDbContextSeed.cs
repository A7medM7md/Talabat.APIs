using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
		{
			if (!userManager.Users.Any())/* Done Just One Time Per Environment */
			{
				var user = new AppUser(){
					DisplayName = "Ahmed Elgebaly",
					Email = "ahmed.mo.elgebaly@gmail.com",
					/*User Must Have Username and Email => So I Can Login With Any One Of Them*/
					UserName = "ahmed.mo.elgebaly",
					PhoneNumber = "01066317907",

				};

				await userManager.CreateAsync(user, "Pa$$w0rd");
			}
		}
	}
}
