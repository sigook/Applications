using Covenant.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Identity;

public class CovenantRoleConfiguration : IEntityTypeConfiguration<CovenantRole>
{
    public void Configure(EntityTypeBuilder<CovenantRole> builder)
    {
        builder.ToTable("Rol");
    }
}
