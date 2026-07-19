using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

public class CompanyInteractionConfiguration : IEntityTypeConfiguration<CompanyInteraction>
{
    public void Configure(EntityTypeBuilder<CompanyInteraction> builder)
    {
        builder.ToTable("CompanyInteraction");
        builder.HasKey(k => k.Id);
        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
