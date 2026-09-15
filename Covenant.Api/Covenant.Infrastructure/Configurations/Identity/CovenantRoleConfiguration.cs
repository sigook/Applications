using Covenant.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Identity;

public class CovenantRoleConfiguration : IEntityTypeConfiguration<CovenantRole>
{
    public void Configure(EntityTypeBuilder<CovenantRole> builder)
    {
        builder.ToTable("Rol");
    }
}
