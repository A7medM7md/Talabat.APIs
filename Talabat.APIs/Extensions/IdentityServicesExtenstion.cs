using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
	public static class IdentityServicesExtenstion
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
		{
			services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				/*Modify Default Identity System Configuration*/
				//options.Password.RequireDigit = true;
				//options.Password.RequireUppercase = true;
			}).AddEntityFrameworkStores<AppIdentityDbContext>();

			services.AddAuthentication(/*JwtBearerDefaults.AuthenticationScheme*/ options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}) // Allow DI For Security Services
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = config["JWT:ValidIssuer"],
					ValidateAudience = true,
					ValidAudience = config["JWT:ValidAudience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"])),
                    ValidateLifetime = true/*Token Has Duration Time Inside It*/
				};

			}); 

			/*Allow DI For TokenService*/;
			services.AddScoped<ITokenService, TokenService>();

			return services;
		}
	}
}
