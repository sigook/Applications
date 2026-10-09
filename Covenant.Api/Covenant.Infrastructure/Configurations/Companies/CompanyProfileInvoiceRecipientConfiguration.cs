using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileInvoiceRecipientConfiguration : IEntityTypeConfiguration<CompanyProfileInvoiceRecipient>
{
    public void Configure(EntityTypeBuilder<CompanyProfileInvoiceRecipient> builder)
    {
        builder.ToTable("CompanyProfileInvoiceRecipients");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.CompanyProfile)
            .WithMany()
            .HasForeignKey(x => x.CompanyProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
