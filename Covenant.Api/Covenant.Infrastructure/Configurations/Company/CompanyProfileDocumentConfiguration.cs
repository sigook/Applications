using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company;

public class CompanyProfileDocumentConfiguration : IEntityTypeConfiguration<CompanyProfileDocument>
{
    public void Configure(EntityTypeBuilder<CompanyProfileDocument> builder)
    {
        builder.ToTable("CompanyProfileDocument");
        builder.HasKey(cpd => new { cpd.DocumentId, cpd.CompanyProfileId });
    }
}
