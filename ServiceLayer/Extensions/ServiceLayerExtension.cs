using EntityLayer.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using ServiceLayer.FluentValidation;
using ServiceLayer.Services.Abstract;
using ServiceLayer.Services.Concrete;
using System.Text;

namespace ServiceLayer.Extensions
{
	public static class ServiceLayerExtension
	{
		public static IServiceCollection LoadServiceLayerExtension(this IServiceCollection services, IConfiguration config)
		{
			// Add Services 
			services.AddScoped<IUserService, UserService>();

			// Add Fluent Validation 
			services.AddFluentValidationAutoValidation(options =>
			{
				options.DisableDataAnnotationsValidation = true;
			});
			services.AddValidatorsFromAssemblyContaining<LoginRequestValidation>();

			// Add Identity & JWT Authentication
			// Identity :
			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddSignInManager()
				.AddDefaultTokenProviders()
				.AddRoles<IdentityRole>();

			// Identity options configuration
			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequiredLength = 7;
			});

			// Token configuration  
			services.Configure<DataProtectionTokenProviderOptions>(opt =>
			{
				opt.TokenLifespan = TimeSpan.FromHours(2); 
			});

			// JWT authentication :
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ValidIssuer = config["Jwt:Issuer"],
					ValidAudience = config["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
				};
			
			});

			return services;
		}
	}
}
