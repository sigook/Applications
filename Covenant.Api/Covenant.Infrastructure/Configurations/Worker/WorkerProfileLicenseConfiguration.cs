using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileLicenseConfiguration : IEntityTypeConfiguration<WorkerProfileLicense>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileLicense> builder)
        {
            builder.ToTable("WorkerProfileLicense");
            builder.HasKey(x => x.Id);
        }
    }
}
