using EntityLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace RepositoryLayer.Configuration
{
	public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
	{
		public void Configure(EntityTypeBuilder<Transaction> builder)
		{
			builder
				.HasKey(x => x.Id);

			builder
				.Property(x => x.Amount)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder
				.Property(x => x.CreatedAt)
				.HasMaxLength(10)
				.IsRequired();

			builder
				.HasOne(x => x.SenderAccount)
				.WithMany(x => x.SentTransactions)
				.HasForeignKey(x => x.SenderAccountNumber)
				.HasPrincipalKey(x => x.AccountNumber)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(x => x.RecipientAccount)
				.WithMany(x => x.ReceivedTransactions)
				.HasForeignKey(x => x.RecipientAccountNumber)
				.HasPrincipalKey(x => x.AccountNumber)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}
