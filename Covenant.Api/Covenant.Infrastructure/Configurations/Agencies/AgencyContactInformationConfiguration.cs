using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class AgencyContactInformationConfiguration : IEntityTypeConfiguration<AgencyContactInformation>
{
    public void Configure(EntityTypeBuilder<AgencyContactInformation> builder)
    {
        builder.ToTable("AgencyContactInformation");
        builder.HasKey(x => x.Id);

        builder.HasOne<Common.Entities.Agency.Agency>()
            .WithMany(x => x.ContactInformation)
            .HasForeignKey("AgencyId");
    }
}
