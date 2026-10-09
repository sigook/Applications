using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileContactPersonConfiguration : IEntityTypeConfiguration<CompanyProfileContactPerson>
{
    public void Configure(EntityTypeBuilder<CompanyProfileContactPerson> builder)
    {
        builder.ToTable("CompanyProfileContactPeople");
        builder.HasKey(k => k.Id);
    }
}
