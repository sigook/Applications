using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Agency
{
    public class AgencyPersonnelConfiguration : IEntityTypeConfiguration<AgencyPersonnel>
    {
        public void Configure(EntityTypeBuilder<AgencyPersonnel> builder)
        {
            builder.ToTable("AgencyPersonnel");
            builder.HasIndex(i => new { i.AgencyId, i.UserId }).IsUnique();
        }
    }
} 