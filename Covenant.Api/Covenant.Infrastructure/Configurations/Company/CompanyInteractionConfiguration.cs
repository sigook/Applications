using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

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
