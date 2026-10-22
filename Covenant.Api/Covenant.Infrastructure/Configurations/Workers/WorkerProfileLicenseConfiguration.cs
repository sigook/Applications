using Covenant.Common.Entities.Workers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileLicenseConfiguration : IEntityTypeConfiguration<WorkerProfileLicense>
{
    public void Configure(EntityTypeBuilder<WorkerProfileLicense> builder)
    {
        builder.ToTable("WorkerProfileLicenses");
        builder.HasKey(x => x.Id);
    }
}
