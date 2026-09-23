using Covenant.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Identity;

public class InactiveUserConfiguration : IEntityTypeConfiguration<InactiveUser>
{
    public void Configure(EntityTypeBuilder<InactiveUser> builder)
    {
        builder.ToTable("InactiveUsers");
        builder.HasKey(iu => iu.InactiveUserId);
    }
}
