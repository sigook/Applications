using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Agency;

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
