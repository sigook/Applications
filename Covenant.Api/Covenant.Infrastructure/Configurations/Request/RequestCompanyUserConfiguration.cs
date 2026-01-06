using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestCompanyUserConfiguration : IEntityTypeConfiguration<RequestCompanyUser>
    {
        public void Configure(EntityTypeBuilder<RequestCompanyUser> builder)
        {
            builder.ToTable("RequestCompanyUsers");
            builder.HasKey(rcu => new { rcu.RequestId, rcu.CompanyUserId });
        }
    }
}
