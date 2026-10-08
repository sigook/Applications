using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileCertificateConfiguration : IEntityTypeConfiguration<WorkerProfileCertificate>
{
    public void Configure(EntityTypeBuilder<WorkerProfileCertificate> builder)
    {
        builder.ToTable("WorkerProfileCertificates");
        builder.HasKey(k => k.Id);
    }
}
