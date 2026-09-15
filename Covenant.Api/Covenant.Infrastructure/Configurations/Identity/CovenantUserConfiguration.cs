using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Identity;

public class CovenantUserConfiguration : IEntityTypeConfiguration<CovenantUser>
{
    public void Configure(EntityTypeBuilder<CovenantUser> builder)
    {
        builder.ToTable("User");
    }
}
