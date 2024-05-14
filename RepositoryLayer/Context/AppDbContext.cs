using EntityLayer.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Context
{
	public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
	{
	}
}
