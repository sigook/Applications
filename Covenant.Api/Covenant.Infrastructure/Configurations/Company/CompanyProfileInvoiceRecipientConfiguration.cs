using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

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
