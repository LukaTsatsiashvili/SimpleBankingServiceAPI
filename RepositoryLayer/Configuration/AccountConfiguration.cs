using EntityLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RepositoryLayer.Configuration
{
	public class AccountConfiguration : IEntityTypeConfiguration<Account>
	{
		public void Configure(EntityTypeBuilder<Account> builder)
		{
			builder
				.HasKey(x => x.Id);

			builder
				.Property(x => x.AccountNumber)
				.IsRequired();

			builder
				.Property(x => x.Balance)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder
				.HasOne(x => x.AppUser)
				.WithMany(x => x.Accounts)
				.HasForeignKey(x => x.AppUserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(x => x.SentTransactions)
				.WithOne(x => x.SenderAccount)
				.HasForeignKey(x => x.SenderAccountNumber)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasMany(x => x.ReceivedTransactions)
				.WithOne(x => x.RecipientAccount)
				.HasForeignKey(x => x.RecipientAccountNumber)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
