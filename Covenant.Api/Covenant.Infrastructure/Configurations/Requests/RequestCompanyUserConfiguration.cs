using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestCompanyUserConfiguration : IEntityTypeConfiguration<RequestCompanyUser>
{
    public void Configure(EntityTypeBuilder<RequestCompanyUser> builder)
    {
        builder.ToTable("RequestCompanyUsers");
        builder.HasKey(rcu => new { rcu.RequestId, rcu.CompanyUserId });
    }
}
