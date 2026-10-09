using Covenant.Common.Entities.Identity;
using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Identity;

public class CovenantUserConfiguration : IEntityTypeConfiguration<CovenantUser>
{
    public void Configure(EntityTypeBuilder<CovenantUser> builder)
    {
        builder.ToTable("User");
    }
}
