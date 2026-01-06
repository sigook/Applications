using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company
{
    public class CompanyProfileContactPersonConfiguration : IEntityTypeConfiguration<CompanyProfileContactPerson>
    {
        public void Configure(EntityTypeBuilder<CompanyProfileContactPerson> builder)
        {
            builder.ToTable("CompanyProfileContactPerson");
            builder.HasKey(k => k.Id);
        }
    }
} 