using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Sales;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Sales;

public class CompanyInteractionConfiguration : IEntityTypeConfiguration<CompanyInteraction>
{
    public void Configure(EntityTypeBuilder<CompanyInteraction> builder)
    {
        builder.ToTable("CompanyInteractions");
        builder.HasKey(k => k.Id);
        builder.HasOne(x => x.CompanyProfile).WithMany().HasForeignKey(x => x.CompanyProfileId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
