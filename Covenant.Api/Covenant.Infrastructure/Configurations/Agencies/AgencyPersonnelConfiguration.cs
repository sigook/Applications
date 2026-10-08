using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class AgencyPersonnelConfiguration : IEntityTypeConfiguration<AgencyPersonnel>
{
    public void Configure(EntityTypeBuilder<AgencyPersonnel> builder)
    {
        builder.ToTable("AgencyPersonnel");
        builder.HasIndex(i => new { i.AgencyId, i.UserId }).IsUnique();
    }
}
