using EntityLayer.DTOs.Auth;
using EntityLayer.Entities.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using ServiceLayer.FluentValidation.Auth;
using ServiceLayer.Helpers;
using ServiceLayer.Middlewares;
using ServiceLayer.Services.API.User.Abstract;
using ServiceLayer.Services.API.User.Concrete;
using ServiceLayer.Services.Auth.Abstract;
using ServiceLayer.Services.Auth.Concrete;
using System.Reflection;
using System.Text;

namespace ServiceLayer.Extensions
{
    public static class ServiceLayerExtension
	{
		public static IServiceCollection LoadServiceLayerExtension(this IServiceCollection services, IConfiguration config)
		{
			// Add All Services Here 

			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IFileValidator, FileValidator>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<ITransactionService, TransactionService>();
			services.AddScoped<IAdminService, AdminService>();

			// Add Fluent Validation 
			services.AddFluentValidationAutoValidation(options =>
			{
				options.DisableDataAnnotationsValidation = true;
			});
			services.AddValidatorsFromAssemblyContaining<LoginRequestValidation>();

			// Add Identity & JWT Authentication
			// Identity 
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

			// Token configuration (we used it for password reset) 
			services.Configure<DataProtectionTokenProviderOptions>(opt =>
			{
				opt.TokenLifespan = TimeSpan.FromHours(2); 
			});

			// JWT authentication 
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

			// Email Settings Mapping & Adding EmailService 
			services.Configure<EmailInformationDTO>(config.GetSection("EmailSettings"));
			services.AddScoped<IEmailSendMethod, EmailSendMethod>();

			// Add 'HttpContextAccessor' for ImageUpload manipulations
			services.AddHttpContextAccessor();

			// Add AutoMapper
			services.AddAutoMapper(Assembly.GetExecutingAssembly());

			return services;
		}

		public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
		{
			return app.UseMiddleware<ExceptionHandlerMiddleware>();
		}
	}
}
