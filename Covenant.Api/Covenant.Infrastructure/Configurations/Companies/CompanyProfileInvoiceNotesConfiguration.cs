using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileInvoiceNotesConfiguration : IEntityTypeConfiguration<CompanyProfileInvoiceNotes>
{
    public void Configure(EntityTypeBuilder<CompanyProfileInvoiceNotes> builder)
    {
        builder.ToTable("CompanyProfileInvoiceNotes");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.CompanyProfile)
            .WithMany()
            .HasForeignKey(x => x.CompanyProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
