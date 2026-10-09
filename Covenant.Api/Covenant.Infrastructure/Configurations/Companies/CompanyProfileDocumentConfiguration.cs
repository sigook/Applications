using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileDocumentConfiguration : IEntityTypeConfiguration<CompanyProfileDocument>
{
    public void Configure(EntityTypeBuilder<CompanyProfileDocument> builder)
    {
        builder.ToTable("CompanyProfileDocuments");
        builder.HasKey(cpd => new { cpd.DocumentId, cpd.CompanyProfileId });
    }
}
