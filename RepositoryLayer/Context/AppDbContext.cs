using EntityLayer.Entities;
using EntityLayer.Entities.Auth;
using EntityLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Context
{
	public class AppDbContext(
		DbContextOptions options,
		IHttpContextAccessor httpContextAccessor) : IdentityDbContext<AppUser>(options)
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<AuditLog> AuditLogs { get; set; }


		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var modifiedEntities = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added
				|| e.State == EntityState.Modified
				|| e.State == EntityState.Deleted)
				.ToList();

			foreach(var modifiedEntity in modifiedEntities)
			{
				AuditLog auditLog = new()
				{
					EntityName = modifiedEntity.Entity.GetType().Name,
					UserEmail = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name),
					Action = modifiedEntity.State.ToString(),
					TimeStamp = DateTime.UtcNow,
					Changes = GetChanges(modifiedEntity)
				};

				AuditLogs.Add(auditLog);
			}

			return base.SaveChangesAsync(cancellationToken);
		}

		private string GetChanges(EntityEntry modifiedEntity)
		{
			var changes = new StringBuilder();

			foreach(var property in modifiedEntity.OriginalValues.Properties)
			{
				var originalValue = modifiedEntity.OriginalValues[property];
				var currentValue = modifiedEntity.CurrentValues[property];

				if (!Equals(originalValue, currentValue))
				{
					changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
				}
			}
			return changes.ToString();
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			base.OnModelCreating(builder);
		}
	}

	
}
