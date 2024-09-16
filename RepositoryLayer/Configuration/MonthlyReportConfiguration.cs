using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RepositoryLayer.Configuration;

public class MonthlyReportConfiguration : IEntityTypeConfiguration<MonthlyReport>
{
	public void Configure(EntityTypeBuilder<MonthlyReport> builder)
	{

		builder
			.HasKey(x => x.Id);

		builder
			.Property(x => x.Id)
			.ValueGeneratedOnAdd();
			
		builder
			.Property(x => x.TotalAmount)
			.HasColumnType("decimal(18,2)");

	}
}
