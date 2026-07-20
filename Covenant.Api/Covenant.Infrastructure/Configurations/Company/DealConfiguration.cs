using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deal");
        builder.HasKey(k => k.Id);
        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Restrict);
    }
}
